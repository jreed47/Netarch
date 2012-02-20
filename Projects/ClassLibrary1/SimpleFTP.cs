using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace ClassLibrary1
{
    public class SimpleFTP : FTP
    {
        public static readonly int BUF_LEN = 65536; //64kb

        public bool sendFile(string hostName, int port, string filePath, ref long time)
        {
            bool resp = false;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            
            TPLayer tp = new UDPLayer();
            if (tp.Connect(hostName, port))
            {
                Console.WriteLine("Error: unable to create connection");
                return true;
            }

            try
            {
                FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                int len = (int)fs.Length; //possible problem if file larger than 4.2Gb
                Console.WriteLine("len={0}", len);

                byte[] lenbytes = BitConverter.GetBytes(len);
                tp.Write(lenbytes, lenbytes.Length);
                //Console.WriteLine("lenbytes={0}", lenbytes.Length);

                byte[] buf = new byte[BUF_LEN];
                for (int ind = 0, ret = 0; ind < len; ind += ret)
                {
                    ret = fs.Read(buf, 0, BUF_LEN);
                    if (tp.Write(buf, ret))
                    {
                        Console.WriteLine("Error: dropped file segment at: {0} of {1}", ind, ret);
                        resp = true;
                        break;
                    }
                }

                //tp.writeByte(buf, ret) //should send EOT or EOF char?
                fs.Close();
            }
            catch (System.IO.IOException e)
            {
                Console.WriteLine(e.Message);
                resp = true;
            }
            tp.Close();
            sw.Stop();

            time = sw.ElapsedMilliseconds;
            return resp;
        }

        public bool recvFile(int port, string filePath, ref long time)
        {
            bool resp = false;
            Stopwatch sw = new Stopwatch();
            
            TPLayer tp = new UDPLayer();
            if (tp.Listen(port))
            {
                Console.WriteLine("Error: unable to create connection");
                return true;
            }

            byte[] buf = new byte[BUF_LEN];

            try
            {
                FileStream fs = File.Open(filePath, FileMode.Create, FileAccess.Write);
                int len = 0;
                int ret = 0;

                byte[] lenbytes = BitConverter.GetBytes(len);
                //Console.WriteLine("lenbytes={0}", lenbytes.Length);
                
                if (tp.Read(buf, lenbytes.Length, ref ret))
                {
                    tp.Close();
                    return true;
                }
                len = BitConverter.ToInt32(buf, 0);
                Console.WriteLine("len={0}", len);
                sw.Start();

                for (int count = len; count > 0; count -= ret)
                {
                    if (count < BUF_LEN)
                    {
                        if (tp.Read(buf, count, ref ret))
                        {
                            Console.WriteLine("Error: dropped file segment at: {0} of {1}", len-count, ret);
                            resp = true;
                            break;
                        }
                    }
                    else
                    {
                        if (tp.Read(buf, BUF_LEN, ref ret))
                        {
                            Console.WriteLine("Error: dropped file segment at: {0} of {1}", len - count, ret);
                            resp = true;
                            break;
                        }
                    }

                    fs.Write(buf, 0, ret);
                }

                fs.Close();
            }
            catch (System.IO.IOException e)
            {
                Console.WriteLine(e.Message);
            }
            tp.Close();
            sw.Stop();

            time = sw.ElapsedMilliseconds;
            return resp;
        }
    }
}
