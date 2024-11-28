using System.Net;
using MCTG._00_Server;

namespace MCTG;

class Program
{
    public static async Task Main(string[] args)
    {
        var server = new TcpServer(IPAddress.Any, 10001);
        await server.StartAsync();
    }
}