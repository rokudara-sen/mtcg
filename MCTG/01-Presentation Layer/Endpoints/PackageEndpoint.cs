using System.Text.Json;
using MCTG._01_Presentation_Layer.Interfaces;
using MCTG._01_Presentation_Layer.Models;
using MCTG._01_Presentation_Layer.Models.Http;
using MCTG._02_Business_Logic_Layer.RouteHandlers;
using MCTG._06_Domain.Entities;

namespace MCTG._01_Presentation_Layer.Endpoints;

public class PackageEndpoint : IEndpoint
{
    private readonly List<Route> _routes;
    private readonly PackageRouteHandler _packageRouteHandler;
    public PackageEndpoint()
    {
        _packageRouteHandler = new PackageRouteHandler();
        _routes = new List<Route>
        {
            new Route("POST", "/packages", HandleCreatePackage),
        };
    }
    public async Task HandleRequest(Request request, Response response)
    {
        var route = FindRoute(request, request.Method, request.Path);
        if (route != null)
        {
            await route.Handler(request, response);
        }
        else
        {
            response.StatusCode = 404;
            response.ReasonPhrase = "Not Found";
            response.Body = "The requested resource was not found.";
        }
    }

    public bool CanHandle(Request request)
    {
        throw new NotImplementedException();
    }

    private Route? FindRoute(Request request, string method, string path)
    {
        foreach (var route in _routes)
        {
            if (string.Equals(route.HttpMethod, method, StringComparison.OrdinalIgnoreCase) &&
                IsMatch(route.PathPattern, path, out var routeParams))
            {
                request.RouteParameters = routeParams;
                return route;
            }
        }
        return null;
    }
    
    private bool IsMatch(string pattern, string path, out Dictionary<string, string> parameters)
    {
        parameters = new Dictionary<string, string>();

        var patternSegments = pattern.Split('/', StringSplitOptions.RemoveEmptyEntries);
        var pathSegments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);

        if (patternSegments.Length != pathSegments.Length)
            return false;

        for (int i = 0; i < patternSegments.Length; i++)
        {
            var patternSegment = patternSegments[i];
            var pathSegment = pathSegments[i];

            if (patternSegment.StartsWith("{") && patternSegment.EndsWith("}"))
            {
                var paramName = patternSegment.Trim('{', '}');
                parameters[paramName] = pathSegment;
            }
            else if (!string.Equals(patternSegment, pathSegment, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
        }
        return true;
    }
    
    private async Task HandleCreatePackage(Request request, Response response)
    {
        var user = JsonSerializer.Deserialize<User>(request.Body);
        
        if (user == null || string.IsNullOrWhiteSpace(user.Authorization))
        {
            response.StatusCode = 400;
            response.ReasonPhrase = "Bad Request";
            return;
        }

        var result = await _packageRouteHandler.CreatePackageAsync(user);

        if (result.Success)
        {
            response.StatusCode = 201;
            response.ReasonPhrase = "OK";
        }
        else
        {
            response.StatusCode = 400;
            response.ReasonPhrase = "Something went wrong";
        }
    }
}