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
        public bool Connect(UserClient userclient)
        {
            //Gets current context of the client CALLING this function
            OperationContext context = OperationContext.Current;
            MessageProperties properties = context.IncomingMessageProperties;
            RemoteEndpointMessageProperty endpoint = properties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
            ICallbackContract callback = OperationContext.Current.GetCallbackChannel<ICallbackContract>();

            userclient.callback = callback;
            userclient.IPAddress = endpoint.Address;

            //ClientData.ConnectedClients.Add(userclient);
            Server.ConnectedClients.Add(userclient);

            Console.WriteLine("Client connected from {0}", userclient.IPAddress);
            Form1.form.listBox1.Items.Add(userclient.Username);

            return true;
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

        //Client-To-Server
        public async Task DownloadAsync(FileTransfer filetransfer)
        {
            try
            {
                Diagnostics.WriteLog("File transfer initiated");
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
                Diagnostics.WriteLog("File transfer complete!");
            }
            catch (Exception ex)
            {
                Diagnostics.WriteLog("File transfer failed!");
                filetransfer.Error = ex.Message;
            }
        }

        //Server-To-Client
        public async Task<FileTransfer> UploadAsync(FileTransfer fileTransfer)
        {
            Stream stream = Stream.Null;
            try
            {
                Diagnostics.WriteLog("File Transfer initiated");
                Server.FileTransfers.Add(fileTransfer);
                stream = File.OpenRead(fileTransfer.ServerFilePath);
                fileTransfer.Data = stream;
                fileTransfer.FileSize = stream.Length;
                return fileTransfer;
            }
            catch (Exception ex)
            {
                Diagnostics.WriteLog("File transfer failed");
                fileTransfer.Error = ex.Message; //cuz why is it empty if its doing this D8
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
    }
}
