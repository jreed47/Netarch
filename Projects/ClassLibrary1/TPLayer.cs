using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClassLibrary1
{
    //Transport Layer Interface
    public abstract class TPLayer
    {
        public abstract bool connect(string hostname, int port);

        public abstract bool listen(int port);

        public abstract bool writeByte(byte data);	//should we change this?

        public abstract bool write(byte[] data, int len);	//should we change this?

        //usage: error = readByte(ref data);
        //public abstract bool readByte(byte data);
        public abstract bool readByte(byte[] data, int index, ref int ret); //TODO: change? figure out better interface?

        public abstract bool read(byte[] data, int len, ref int ret);

        public abstract bool close();
    }
}
