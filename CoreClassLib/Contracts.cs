using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace KynetLib
{
    [ServiceContract(CallbackContract = typeof(ICallbackContract), SessionMode = SessionMode.Required)]
    public interface IContract
    {
        [OperationContract]
        bool Connect(UserClient userclient);
        [OperationContract]
        bool Disconnect();
        [OperationContract]
        void Message(string message);

    }

    [ServiceContract]
    public interface ICallbackContract
    {
        [OperationContract(IsOneWay = true)] 
        void Message(string Message);
        [OperationContract(IsOneWay = true)]
        void DownloadAsync(string filepath);
        [OperationContract(IsOneWay = true)]
        void UploadAsync(string clientFilePath, string serverFilePath); 
    }

    [ServiceContract]
    public interface IFileTransferContract
    {
        //Downloads from Client To Server
        [OperationContract]
        Task DownloadAsync(FileTransfer fileTransfer);

        //Uploads from Server to client
        [OperationContract]
        Task<FileTransfer> UploadAsync(FileTransfer fileTransfer);

        //If the client was unable to transfer a file from or to the server
        [OperationContract]
        void ReportTransferError(FileTransfer fileTransfer);
    }
}
