using System.Net;
using System.Net.Sockets;
namespace MCTG._00_Server;

public class TcpServer
{
    private readonly TcpListener _tcpListener;
    private readonly HttpProcessor _httpProcessor;

    public TcpServer(IPAddress ipAddress, int port)
    {
        _tcpListener = new TcpListener(ipAddress, port);
        _httpProcessor = new HttpProcessor();
    }

    public void Start()
    {
        Console.WriteLine("Listening for connections...");

        _tcpListener.Start();
        
        while (true)
        {
            try
            {
                var clientSocket = _tcpListener.AcceptTcpClient();
                var thread = new Thread(() => ProcessRequest(clientSocket));
                thread.Start();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }
    }

    private void ProcessRequest(TcpClient clientSocket)
    {
        _httpProcessor.ProcessRequest(clientSocket);
    }
}