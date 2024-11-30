using MTCG._01_Shared;
using MTCG._02_Business_Logic_Layer.Interfaces;
using MTCG._02_Business_Logic_Layer.RouteHandlers;
using MTCG._01_Presentation_Layer.Endpoints;
using MTCG._01_Presentation_Layer.Interfaces;
using MTCG._01_Presentation_Layer.Models.Http;

namespace MTCG._02_Business_Logic_Layer.Services;

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