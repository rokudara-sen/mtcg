using MCTG._01_Presentation_Layer.Interfaces;
using MCTG._01_Presentation_Layer.Models;
using MCTG._01_Presentation_Layer.Models.Http;
using MCTG._02_Business_Logic_Layer.RouteHandlers;

namespace MCTG._01_Presentation_Layer.Endpoints;

public class CardEndpoint : IEndpoint
{
    private readonly List<Route> _routes;
    private readonly UserRouteHandler _userRouteHandler;

    public CardEndpoint()
    {
        _userRouteHandler = new UserRouteHandler();
        _routes = new List<Route>
        {
            new Route("POST", "/users", HandleRegisterUser),
            new Route("POST", "/sessions", HandleLoginUser),
            new Route("GET", "/users/{username}", HandleUpdateUser),
        };
    }
    public Task HandleRequest(Request request, Response response)
    {
        throw new NotImplementedException();
    }

    public bool CanHandle(Request request)
    {
        throw new NotImplementedException();
    }
}