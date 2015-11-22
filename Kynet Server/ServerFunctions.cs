using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KynetLib;
using System.IO;
using KynetServer.Web;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;

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
        public static void ReceivedClientCmdOutput()
        {

        }

        public static async Task<CountryInformation> GetCountryInfo(string IpAddress)
        {
            using (HttpClient client = new HttpClient())
            {
                string json = await client.GetStringAsync(new Uri(WebVars.GEO_IP_API_ADDRESS + IpAddress));
                CountryInformation countryInfo = new CountryInformation();
                JObject JsonObject = JObject.Parse(json);
                countryInfo.Name = (string)JsonObject.SelectToken("country_name");
                countryInfo.Code = (string)JsonObject.SelectToken("region_code");
                countryInfo.City = (string)JsonObject.SelectToken("city");
                countryInfo.Region = (string)JsonObject.SelectToken("region_name");
                countryInfo.TimeZone = (string)JsonObject.SelectToken("time_zone");
                return countryInfo;
            }
        }
    }
}
