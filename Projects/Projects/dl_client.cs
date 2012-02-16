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
            long time = 0;
            bool ret = ftp.sendFile(args[0], port, args[2], ref time);

            if (ret)
            {
                Console.WriteLine("Transfer Error");
            }
            else
            {
                FileStream fs = new FileStream(args[2], FileMode.Open, FileAccess.Read);
                long len = fs.Length; //possible problem if file larger than 4.2Gb
                Console.WriteLine("Transmitted file of size: {0}, over time: {1}, at rate: {2} kbps", len, time, len / (double)time);
            }
        }
    }
}
