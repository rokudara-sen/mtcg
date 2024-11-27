using MCTG._01_Presentation_Layer.Endpoints;
using MCTG._01_Presentation_Layer.Interfaces;
using MCTG._01_Presentation_Layer.Models.Http;
using MCTG._01_Shared;
using MCTG._02_Business_Logic_Layer.Interfaces;
using MCTG._02_Business_Logic_Layer.RouteHandlers;

namespace MCTG._02_Business_Logic_Layer.Services;

public class Router
{
    private readonly List<IEndpoint> _endpoints;

    public Router()
    {
        _endpoints = new List<IEndpoint>
        {
            new UserEndpoint(),
        };
    }
    public async Task RouteRequest(Request request, Response response)
    {
        foreach (var endpoint in _endpoints)
        {
            if (endpoint.CanHandle(request))
            {
                await endpoint.HandleRequest(request, response);
                return;
            }
        }
        
        response.StatusCode = 404;
        response.ReasonPhrase = "Not Found";
        response.Body = "The requested resource was not found.";
    }
}