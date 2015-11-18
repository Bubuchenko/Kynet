using KynetLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KynetClient
{
    public class Reporting
    {
        //Report an event to the server
        public static void ReportEvent(string message, EventType type, Exception ex = null, params object[] additionalobjects)
        {
            Task.Factory.StartNew(() => { 
                UserEvent e = new UserEvent();
                e.Fingerprint = System.SystemInfo.Fingerprint;
                e.ExceptionMessage = ex != null ? ex.Message : "";
                e.Message = message;
                e.Type = type;

                if (type == EventType.FileTransferError)
                {
                    FileTransfer f = additionalobjects.First() as FileTransfer;
                    f.Error = message;
                    Client.channel.SendFileTransferEvent(f);
                }

                Client.channel.SendEvent(e);
            });
        }
    }
}
