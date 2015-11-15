using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using KynetLib;

namespace KynetClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Client chadcat = new Client();
            chadcat.Connect();
            while(true)
            {
                Console.WriteLine("Enter message:");
                string message = Console.ReadLine();
                chadcat.channel.Message(message);

            }
        }
    }
}
