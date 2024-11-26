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

    public void ProcessRequest(TcpClient clientSocket)
    {
        using var networkStream = clientSocket.GetStream();
        using var reader = new StreamReader(networkStream);
        using var writer = new StreamWriter(networkStream) { AutoFlush = true };

        var request = _requestHandler.ReadRequest(reader);
        var response = new Response();

        var routeHandler = _router.GetHandler(request.Path);

        if (routeHandler != null)
        {
            routeHandler.HandleRequest(request, response);
        }
        else
        {
            Console.WriteLine($"No handler found for path: {request.Path}");
            response.StatusCode = 404;
            response.ReasonPhrase = "Not Found";
            response.Body = "The requested resource was not found.";
        }

        _responseHandler.SendResponse(writer, response);
    }
}