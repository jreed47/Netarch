using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ClassLibrary1;

namespace server
{
    class dl_server
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Usage: dl_server <port>");
                return;
            }
            int port;
            Int32.TryParse(args[0], out port);

            FTP ftp = new SimpleFTP();
            string filePath = "output.dat";

            long time = 0;
            bool ret = ftp.recvFile(port, filePath, ref time);
            
            if (ret)
            {
                Console.WriteLine("Transfer Error");
            }
            else
            {
                FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                long len = fs.Length; //possible problem if file larger than 4.2Gb
                Console.WriteLine("Received file of size: {0}, over time: {1}, at rate: {2} kbps", len, time, len / (double)time);
            }
        }
    }
}
