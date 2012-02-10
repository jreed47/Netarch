using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClassLibrary1;

namespace client
{
    public class dl_client
    {
        // Main begins program execution.
        static void Main(string[] args)
        {
            if (args.Length != 4)
            {
                Console.WriteLine("Usage: dl_client <hostname> <port> <filename>");
                return;
            }
            int port;
            System.Int32.TryParse(args[2], out port);

            TPLayer tp = new UDPLayer();
            if (tp.connect(args[1], port))
            {
                Console.WriteLine("Error: unable to create connection");
                Environment.Exit(0);
            }


            try
            {
                byte[] filebytes = System.IO.File.ReadAllBytes(args[3]);

                int len = filebytes.Length;
                byte[] lenbytes = System.BitConverter.GetBytes(len);

                foreach (byte b in lenbytes)
                {
                    tp.writeByte(b);
                }
                //tp.write(lenbytes, lenbytes.Length());	

                foreach (byte b in filebytes)
                {
                    tp.writeByte(b);
                }
                //tp.write(filebytes, len);	
            }
            catch (System.IO.IOException e)
            {
                Console.WriteLine(e.Message);
            }
            tp.close();
        }
    }
}
