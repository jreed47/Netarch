using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClassLibrary1
{
    public interface FTP
    {
        bool sendFile(string hostName, int port, string filePath, ref long time);

        bool recvFile(int port, string filePath, ref long time);
    }
}
