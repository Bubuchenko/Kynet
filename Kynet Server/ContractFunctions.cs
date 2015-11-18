using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using KynetLib;
using System.Diagnostics;
using System.Threading;

namespace KynetServer
{
    class ContractFunctions : IContract, IFileTransferContract
    {
        public string Connect(ClientSystemInfo systemInfo)
        {
            //Gets current context of the client CALLING this function
            OperationContext context = OperationContext.Current;
            MessageProperties properties = context.IncomingMessageProperties;
            RemoteEndpointMessageProperty endpoint = properties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
            ICallbackContract callback = OperationContext.Current.GetCallbackChannel<ICallbackContract>();

            UserClient client = new UserClient();
            client.System = systemInfo;
            client.callback = callback;
            client.IPAddress = endpoint.Address;

            Server.ConnectedClients.Add(client);

            Console.WriteLine("Client connected from {0}", client.IPAddress);
            Form1.form.listBox1.Items.Add(client.System.Username);

            return client.Fingerprint;
        }

        public bool Disconnect()
        {
            return false;
        }
        public void Message(string message)
        {
            Task.Factory.StartNew(() =>
                Diagnostics.WriteLog(message)
            );
        }

        public void SendEvent(UserEvent userEvent)
        {
            EventManagement.HandleEvent(userEvent);
        }
        public void SendFileTransferEvent(FileTransfer fileInfo)
        {
            EventManagement.HandleTransferError(fileInfo);
        }

        //Client-To-Server
        public async Task DownloadAsync(FileTransfer filetransfer)
        {
            try
            {
                Server.FileTransfers.Add(filetransfer);
                Stopwatch sw = new Stopwatch();
                sw.Start();
                using (Stream output = File.Create(filetransfer.FileName))
                {
                    byte[] buffer = new byte[4 * 1024];
                    int len;
                    while ((len = await Task.Run(() => filetransfer.Data.ReadAsync(buffer, 0, buffer.Length))) > 0)
                    {
                        filetransfer.Duration = sw.Elapsed;
                        await Task.Run(() => output.WriteAsync(buffer, 0, len));
                    }
                    sw.Stop();
                }
            }
            catch (Exception ex)
            {
                filetransfer.Error = ex.Message;
            }
        }

        //Server-To-Client
        public async Task<FileTransfer> UploadAsync(FileTransfer fileTransfer)
        {
            await Task.Delay(0);
            try
            {
                Server.FileTransfers.Add(fileTransfer);
                fileTransfer.Data = File.OpenRead(fileTransfer.ServerFilePath);
                fileTransfer.FileSize = fileTransfer.Data.Length;
                return fileTransfer;
            }
            catch (Exception ex)
            {
                fileTransfer.Error = ex.Message;
                return fileTransfer;
            }
        }

        public void ReportTransferError(FileTransfer fileTransfer)
        {
            //Check if it already exists
            FileTransfer fileTransferObject = Server.FileTransfers.Where(f => f.ID == fileTransfer.ID).FirstOrDefault();

            //If not, add it
            if(fileTransferObject == null)
            {
                Server.FileTransfers.Add(fileTransfer);
            }
            else //otherwise replace the old with the new one
            {
                fileTransferObject = fileTransfer;
            }
        }

        public void ReceiveClientCmdOutput(string output)
        {

        }
    }
}
