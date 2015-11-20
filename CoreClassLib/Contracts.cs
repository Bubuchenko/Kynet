using System;
using System.Collections.Generic;
using System.Collections;
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
        string Connect(ClientSystemInfo userclient);
        [OperationContract]
        bool Disconnect();
        [OperationContract]
        void Message(string message);
        [OperationContract]
        void SendEvent(UserEvent userEvent);
        [OperationContract]
        void SendFileTransferEvent(FileTransfer fileInfo);
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
        [OperationContract]
        Task<DirectoryInformation> GetFolderStructure(string directoryPath);
        [OperationContract]
        Task<List<string>> ExecuteRemoteCommand(string command);
        [OperationContract]
        Task<List<UserProcess>> GetProcesses();


        void OpenFile(string filepath, string arguments);
        void RemoveFiles(string[] filepath);
        void RenameFile(string filepath, string newName);
        void MoveFile(string filepath, string newPath);

        void OpenFolder(string Folderpath);
        void RemoveFolders(string[] Folderpath);
        void RenameFolder(string Folderpath, string newName);
        void MoveFolder(string Folderpath, string newPath);
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
