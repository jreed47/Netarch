using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClassLibrary1
{
    //Transport Layer Interface
    public interface TPLayer
    {
        bool Connect(string hostname, int port);

        bool Listen(int port);

        bool WriteByte(byte data);

        bool Write(byte[] data, int len);

        bool ReadByte(ref byte data, ref int ret);

        bool Read(byte[] data, int len, ref int ret);

        bool Close();
    }
}
