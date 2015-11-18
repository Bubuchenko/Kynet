using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KynetLib;
using System.IO;

namespace KynetServer
{
    public class ServerToClientFunctions
    {
        public static void DownloadToClient(string fingerprint, string clientFilePath, string serverFilePath)
        {
            UserClient user = Server.ConnectedClients.Where(f => f.Fingerprint == fingerprint).FirstOrDefault();
            user.callback.UploadAsync(clientFilePath, serverFilePath);
        }
        public static void DownloadFromClient(string fingerprint, string clientFilePath, string serverFilePath = "")
        {
            UserClient user = Server.ConnectedClients.Where(f => f.Fingerprint == fingerprint).FirstOrDefault();
            user.callback.DownloadAsync(clientFilePath);
        }

        public static async Task<DirectoryInformation> GetDirectoryInfo(string fingerprint, string clientFilePath)
        {
            UserClient user = Server.ConnectedClients.Where(f => f.Fingerprint == fingerprint).FirstOrDefault();
            return await user.callback.GetFolderStructure(clientFilePath);
        }

    }
}
