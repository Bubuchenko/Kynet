using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KynetLib
{
    public class Diagnostics
    {
        public static void WriteError(Exception ex)
        {
            Console.WriteLine("{0:HH:mm:ss tt}: {1}", DateTime.Now, ex.Message);
        }
        public static void WriteLog(string message)
        {
            Console.WriteLine("{0:HH:mm:ss tt}: {1}", DateTime.Now, message);
        }
    }
}
