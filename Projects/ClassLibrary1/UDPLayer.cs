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
        private EndPoint localEP;

        public static readonly int BUF_LEN = 500;
        private byte[] send_buf;
        private int send_len;
        private byte[] recv_buf;
        private int recv_len;
        private int recv_ind;

        public UDPLayer()
        {
            if (Socket.OSSupportsIPv6)
            {
                sock = new Socket(AddressFamily.InterNetworkV6, SocketType.Dgram, ProtocolType.Udp);
                sock.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.IPv6Only, 0);
            }
            else
            {
                sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            }

            send_buf = new byte[BUF_LEN];
            send_len = 0;

            recv_buf = new byte[BUF_LEN];
            recv_ind = 0;
            recv_len = 0;
        }

        public bool Connect(string hostname, int port)
        {
            try
            {
                IPHostEntry hostInfo = Dns.GetHostEntry(hostname);
                IPAddress[] IPaddrList = hostInfo.AddressList;

                IPAddress hostAddr = null;
                IPEndPoint testEP = null;
                hostEP = null;

                if (Socket.OSSupportsIPv6)
                {
                    for (int i = 0; i < IPaddrList.Length; i++)
                    {
                        hostAddr = IPaddrList[i];
                        testEP = new IPEndPoint(hostAddr, port);

                        if (testEP.AddressFamily == AddressFamily.InterNetworkV6)
                        {
                            //test
                            hostEP = testEP;
                            return false;
                        }
                    }
                }

                for (int i = 0; i < IPaddrList.Length; i++)
                {
                    hostAddr = IPaddrList[i];
                    hostEP = new IPEndPoint(hostAddr, port);

                    if (testEP.AddressFamily == AddressFamily.InterNetwork)
                    {
                        //test
                        hostEP = testEP;
                        return false;
                    }
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine(" Exception {0}", exc.Message);
            }
            return true;
        }

        public bool Listen(int port)
        {
            try
            {
                IPEndPoint client = new IPEndPoint((Socket.OSSupportsIPv6 ? IPAddress.IPv6Any : IPAddress.Any), port);
                localEP = (EndPoint)client;

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
                /*
                //######
                System.Text.Encoding enc = System.Text.Encoding.ASCII;
                string myString = enc.GetString(send_buf, 0, send_len);
                Console.WriteLine("sending={0}", myString);
                Console.WriteLine("length={0}", send_len);
                //######
                */
                sock.SendTo(send_buf, send_len, SocketFlags.None, hostEP);
            }
            catch (Exception exc)
            {
                Console.WriteLine(" Exception {0}", exc.Message);
                return true;
            }
            return false;
        }

        public bool WriteByte(byte data)
        {
            send_buf[send_len] = data;
            send_len++;
            if (send_len == BUF_LEN)
            {
                if (send())
                {
                    return true;
                }
                send_len = 0;
            }
            return false;
        }

        public bool Write(byte[] data, int len)
        {
            for (int i = 0; i < len; i++)
            {
                if (WriteByte(data[i]))
                {
                    return true;
                }
            }
            return false;
        }

        private bool recv()
        {
            try
            {
                recv_len = sock.ReceiveFrom(recv_buf, BUF_LEN, SocketFlags.None, ref localEP);
                /*
                //######
                System.Text.Encoding enc = System.Text.Encoding.ASCII;
                string myString = enc.GetString(recv_buf, 0, recv_len);
                Console.WriteLine("recv={0}", myString);
                Console.WriteLine("length={0}", recv_len);
                //######
                 */
            }
            catch (Exception exc)
            {
                Console.WriteLine(" Exception {0}", exc.Message);
                return true;
            }
            return false;
        }

        public bool ReadByte(ref byte data, ref int ret)
        {
            if (recv_ind == recv_len)
            {
                if (0 < recv_len && recv_len < BUF_LEN)
                {
                    ret = 0;
                    return false;
                }
                if (recv())
                {
                    ret = 0;
                    return true;
                }
                recv_ind = 0;
            }
            data = recv_buf[recv_ind];
            recv_ind++;

            ret = 1;
            return false;
        }

        public bool Read(byte[] data, int len, ref int ret)
        {
            int temp = 0;
            for (int i = 0; i < len; i++)
            {
                if (ReadByte(ref data[i], ref temp))
                {
                    ret = i;
                    return true;
                }
                if (temp == 0)
                {
                    ret = i;
                    return false;
                }
            }
            ret = len;
            return false;
        }

        public bool Close()
        {
            if (send_len > 0)
            {
                if (send())
                {
                    return true;
                }
                send_len = 0;
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
