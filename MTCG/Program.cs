using System.Net;
using MTCG._00_Server;

namespace MTCG;

class Program
{
    public static async Task Main(string[] args)
    {
        var server = new TcpServer(IPAddress.Any, 10001);
        await server.StartAsync();
    }
}