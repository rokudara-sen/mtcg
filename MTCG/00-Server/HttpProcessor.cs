using System.Net.Sockets;
using MTCG._01_Presentation_Layer.Models.Http;
using MTCG._02_Business_Logic_Layer.Services;

namespace MTCG._00_Server
{
    public class HttpProcessor
    {
        private readonly HttpRequest _requestHandler;
        private readonly HttpResponse _responseHandler;
        private readonly Router _router;

        public HttpProcessor(Router router)
        {
            _router = router ?? throw new ArgumentNullException(nameof(router));
            _requestHandler = new HttpRequest();
            _responseHandler = new HttpResponse();
        }

        public async Task ProcessRequest(TcpClient clientSocket)
        {
            await using var networkStream = clientSocket.GetStream();
            using var reader = new StreamReader(networkStream);
            await using var writer = new StreamWriter(networkStream);
            writer.AutoFlush = true;

            var request = await _requestHandler.ReadRequestAsync(reader);
            var response = new Response();

            await _router.RouteRequest(request, response);

            await _responseHandler.SendResponseAsync(writer, response);
        }
    }
}