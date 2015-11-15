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

        public void Upload(string filepath)
        {
            
        }
    }
}
