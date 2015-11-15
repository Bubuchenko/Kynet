using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KynetLib;
using System.IO;
using System.Threading;

namespace KynetClient
{
    public class CallbackContractFunctions : ICallbackContract
    {
        public void Message(string message)
        {
            Diagnostics.WriteLog(message);
        }


        public async void DownloadAsync(string FilePath)
        {
            FileTransfer fileTranferInfo = new FileTransfer();

            try
            {
                fileTranferInfo.Fingerprint = Local.UserInfo.Fingerprint;
                fileTranferInfo.transferType = FileTransfer.TransferType.Download;
                fileTranferInfo.Data = Stream.Null;
                string filename = Path.GetFileName(FilePath);
                fileTranferInfo.ClientFilePath = Path.GetFullPath(filename);
                fileTranferInfo.FileName = filename;

                using (Stream stream = File.OpenRead(FilePath))
                {
                    Diagnostics.WriteLog(string.Format("Download {0} started", FilePath));
                    fileTranferInfo.Data = stream;
                    fileTranferInfo.FileSize = stream.Length;
                    await Task.Run(() => Client.FileserviceClient.CreateChannel().DownloadAsync(fileTranferInfo));
                    Diagnostics.WriteLog(string.Format("Download {0} finished", FilePath));
                }
            }
            catch (Exception ex)
            {
                fileTranferInfo.Error = ex.Message;
                Client.FileserviceClient.CreateChannel().ReportTransferError(fileTranferInfo);
            }
        }

        public async void UploadAsync(string clientFilePath, string serverFilePath)
        {
            FileTransfer fileTransferinfo = new FileTransfer();
            try
            {
                fileTransferinfo.Fingerprint = Local.UserInfo.Fingerprint;
                fileTransferinfo.FileName = Path.GetFileName(serverFilePath);
                fileTransferinfo.transferType = FileTransfer.TransferType.Upload;
                fileTransferinfo.Data = Stream.Null;
                fileTransferinfo.ClientFilePath = clientFilePath;
                fileTransferinfo.ServerFilePath = serverFilePath;

                FileTransfer fileTransferResponse = await Client.FileserviceClient.CreateChannel().UploadAsync(fileTransferinfo);

                if (fileTransferResponse.Error == null)
                {
                    Diagnostics.WriteLog(string.Format("File download started: {0}", fileTransferinfo.FileName));
                    using (Stream output = File.Create(fileTransferinfo.FileName))
                    {
                        byte[] buffer = new byte[4 * 1024];
                        int len;
                        while ((len = await Task.Run(() => fileTransferResponse.Data.ReadAsync(buffer, 0, buffer.Length))) > 0)
                        {
                            await Task.Run(() => output.WriteAsync(buffer, 0, len));
                        }
                    }
                }
                else
                {
                    Diagnostics.WriteLog("File download has failed!");
                    Client.FileserviceClient.CreateChannel().ReportTransferError(fileTransferinfo);
                }
            }
            catch (Exception ex)
            {
                Diagnostics.WriteError(ex);
                fileTransferinfo.Error = ex.Message;
                Client.FileserviceClient.CreateChannel().ReportTransferError(fileTransferinfo);
            }
        }
    }
}
