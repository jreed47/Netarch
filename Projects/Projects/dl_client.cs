using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ClassLibrary1;

namespace client
{
    public class dl_client
    {
        // Main begins program execution.
        static void Main(string[] args)
        {
            if (args.Length != 3)
            {
                Console.WriteLine("Usage: dl_client <hostname> <port> <filename>");
                return;
            }
            int port;
            Int32.TryParse(args[1], out port);

            FTP ftp = new SimpleFTP();
            ftp.sendFile(args[0], port, args[2]);
        }
    }
}
