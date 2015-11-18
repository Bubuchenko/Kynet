using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KynetLib;

namespace KynetServer
{
    class EventManagement
    {
        public static void HandleEvent(UserEvent userEvent)
        {
            UserClient client = Server.ConnectedClients.Where(f => f.Fingerprint == userEvent.Fingerprint).FirstOrDefault();
            client.Events.Add(userEvent);

            switch (userEvent.Type)
            {
                case EventType.FileTransferError:
                    break;
                case EventType.EventNotification:
                    break;
                case EventType.GeneralError:
                    break;
            }
        }

        public static void HandleTransferError(FileTransfer fileTransfer)
        {
            //Check if it already exists
            FileTransfer fileTransferObject = Server.FileTransfers.Where(f => f.ID == fileTransfer.ID).FirstOrDefault();

            //If not, add it
            if (fileTransferObject == null)
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
