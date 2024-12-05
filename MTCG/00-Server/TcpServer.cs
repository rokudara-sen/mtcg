using System.Net;
using System.Net.Sockets;

namespace MTCG._00_Server
{
    public class TcpServer
    {
        private readonly TcpListener _tcpListener;
        private readonly HttpProcessor _httpProcessor;

        public TcpServer(IPAddress ipAddress, int port, HttpProcessor httpProcessor)
        {
            _tcpListener = new TcpListener(ipAddress, port);
            _httpProcessor = httpProcessor ?? throw new ArgumentNullException(nameof(httpProcessor));
        }

        public async Task StartAsync()
        {
            Console.WriteLine("Listening for connections...");

            _tcpListener.Start();

            while (true)
            {
                try
                {
                    var clientSocket = await _tcpListener.AcceptTcpClientAsync();
                    _ = Task.Run(() => ProcessRequestAsync(clientSocket));
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.Message);
                }
            }
        }

        private async Task ProcessRequestAsync(TcpClient clientSocket)
        {
            try
            {
                await _httpProcessor.ProcessRequest(clientSocket);
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Error processing request: {exception.Message}");
                Console.WriteLine(exception.StackTrace);
            }
            finally
            {
                clientSocket.Close();
            }
        }
    }
}