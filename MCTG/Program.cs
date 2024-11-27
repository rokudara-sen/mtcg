using System.Net;
using MCTG._00_Server;

namespace MCTG;

class Program
{
    static void Main(string[] args)
    {
        var tcpServer = new TcpServer(IPAddress.Loopback, 10001);
        tcpServer.Start();
    }
}