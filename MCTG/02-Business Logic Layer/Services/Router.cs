using MCTG._01_Shared;
using MCTG._02_Business_Logic_Layer.Interfaces;
using MCTG._02_Business_Logic_Layer.RouteHandlers;

namespace MCTG._02_Business_Logic_Layer.Services;

public class Router
{
    private readonly Dictionary<string, IRouteHandler> _routeHandlers;

    public Router()
    {
        _routeHandlers = new Dictionary<string, IRouteHandler>
        {
            { "/users", new UserRouteHandler() },
            { "/sessions", new UserRouteHandler() }
        };
    }
    public IRouteHandler GetHandler(string path)
    {
        return _routeHandlers.ContainsKey(path) ? _routeHandlers[path] : null;
    }
}