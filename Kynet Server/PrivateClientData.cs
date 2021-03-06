﻿using KynetLib;
using KynetServer.Web;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KynetServer
{
    public class UserClient
    {
        public ClientSystemInfo System { get; set; }
        public CountryInformation CountryInfo { get; set; }
        public string IPAddress { get; set; }
        public string Username
        {
            get
            {
                if (System != null)
                    return System.Username;
                else
                    return "None";
            }
        }
        public string Fingerprint
        {
            get
            {
                return string.Format("{0}-{1}{2}", Username.Replace(" ", ""), System.DRIVEID, System.CPUID);
            }
        }
        [JsonIgnore]
        public ICallbackContract callback { get; set; }
        public BindingList<UserEvent> Events = new BindingList<UserEvent>();
    }
}
