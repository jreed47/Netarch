using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace ClassLibrary1
{
    //UDPLayer.cs
    public class UDPLayer : TPLayer
    {
        private Socket sock = null;
        private IPEndPoint hostEP;
        //private IPEndPoint localEP;
        private EndPoint localEP;

        public static int BUF_LEN = 500;
        private byte[] send_buf;
        private int send_ind;
        private byte[] recv_buf;
        private int recv_ind;

        public UDPLayer()
        {
            sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            send_buf = new byte[BUF_LEN];
            send_ind = 0;

            recv_buf = new byte[BUF_LEN];
            recv_ind = 0;
        }

        public override bool connect(string hostname, int port)
        {
            IPHostEntry hostInfo = Dns.GetHostEntry(hostname);
            IPAddress[] IPaddrList = hostInfo.AddressList;

            IPAddress hostAddr = null;
            for (int i = 0; i < IPaddrList.Length; i++)
            {
                hostAddr = IPaddrList[i];
                hostEP = new IPEndPoint(hostAddr, port);

                //TODO: finish
                //Socket.SupportsIPv6
                //Socket.SupportsIPv4
                //currAddr.AddressFamily.ToString() == ProtocolFamily.InterNetworkV6.ToString()

                //verify fine
            }
            return false;
        }

        public override bool listen(int port)
        {
            //localEP = new IPEndPoint(IPAddress.Any, port);
            IPEndPoint client = new IPEndPoint(IPAddress.Any, port);
            localEP = (EndPoint)client;

            try
            {
                sock.Bind(localEP);
                //TODO: finish (?)
            }
            catch (Exception exc)
            {
                Console.WriteLine(" Exception {0}", exc.Message);
                return true;
            }
            return false;
        }

        private bool send()
        {
            try
            {
                sock.SendTo(send_buf, BUF_LEN, SocketFlags.None, hostEP);
            }
            catch (Exception exc)
            {
                Console.WriteLine(" Exception {0}", exc.Message);
                return true;
            }
            return false;
        }

        public override bool writeByte(byte data)
        {
            send_buf[send_ind] = data;
            send_ind++;
            if (send_ind == BUF_LEN)
            {
                send_ind = 0;
                if (send())
                {
                    return true;
                }
            }
            return false;
        }

        public override bool write(byte[] data, int len)
        {
            for (int i = 0; i < len; i++)
            {
                if (writeByte(data[i]))
                {
                    return true;
                }
            }
            return false;
        }

        //usage: error = recv(ref len);
        private bool recv(ref int len)
        {
            try
            {
                len = sock.ReceiveFrom(recv_buf, BUF_LEN, SocketFlags.None, ref localEP);
            }
            catch (Exception exc)
            {
                Console.WriteLine(" Exception {0}", exc.Message);
                return true;
            }
            return false;
        }

        public override bool read(byte[] data, int len, ref int ret)
        {
            bool msg_end = false;
            int recv_len = 0;
            for (int i = 0; i < len; i++)
            {
                if (recv_ind == 0)
                {
                    if (msg_end || recv(ref recv_len))
                    {
                        ret = i;
                        return false;
                    }
                    msg_end = recv_len != BUF_LEN;
                }
                data[i] = recv_buf[BUF_LEN - recv_ind];
                recv_ind--;
            }
            ret = len;
            return false;
        }

        public override bool readByte(byte[] data, int index, ref int ret)
        {
            byte[] temp = new byte[1];
            if (read(temp, 1, ref ret))
            {
                return true;
            }
            return false;
        }


        public override bool close()
        {
            if (send_ind > 0)
            {
                send_ind = 0;
                send();
            }

            try
            {
                sock.Close();
            }
            catch (Exception exc)
            {
                Console.WriteLine(" Exception {0}", exc.Message);
                return true;
            }

            return false;
        }
    }
}
