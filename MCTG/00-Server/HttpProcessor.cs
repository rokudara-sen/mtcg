using System.Net.Sockets;
using MCTG._01_Presentation_Layer.Models.Http;
using MCTG._02_Business_Logic_Layer.Services;

namespace MCTG._00_Server;

public class HttpProcessor
{
    private readonly HttpRequest _requestHandler;
    private readonly HttpResponse _responseHandler;
    private readonly Router _router;

    public HttpProcessor()
    {
        _router = new Router();
        _requestHandler = new HttpRequest();
        _responseHandler = new HttpResponse();
    }

    public async Task ProcessRequest(TcpClient clientSocket)
    {
        using var networkStream = clientSocket.GetStream();
        using var reader = new StreamReader(networkStream);
        using var writer = new StreamWriter(networkStream);
        writer.AutoFlush = true;

        var request = _requestHandler.ReadRequest(reader);
        var response = new Response();

        await _router.RouteRequest(request, response);

        _responseHandler.SendResponse(writer, response);
    }
}