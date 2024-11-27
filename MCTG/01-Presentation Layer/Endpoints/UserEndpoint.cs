using System.Collections.Concurrent;
using System.Text.Json;
using MCTG._01_Presentation_Layer.Interfaces;
using MCTG._01_Presentation_Layer.Models;
using MCTG._01_Presentation_Layer.Models.Http;
using MCTG._02_Business_Logic_Layer.Interfaces;
using MCTG._02_Business_Logic_Layer.RouteHandlers;
using MCTG._06_Domain.Entities;

namespace MCTG._01_Presentation_Layer.Endpoints;

public class UserEndpoint : IEndpoint
{
    private readonly List<Route> _routes;
    private readonly UserRouteHandler _userRouteHandler;

    public UserEndpoint()
    {
        _routes = new List<Route>
        {
            new Route("POST", "/users", HandleRegisterUser),
            new Route("POST", "/sessions", HandleLoginUser),
            new Route("GET", "/users/{username}", HandleUpdateUser),
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

    private Route FindRoute(Request request, string method, string path)
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
    
    public bool CanHandle(Request request)
    {
        foreach (var route in _routes)
        {
            if (string.Equals(route.HttpMethod, request.Method, StringComparison.OrdinalIgnoreCase) &&
                IsMatch(route.PathPattern, request.Path, out _))
            {
                return true;
            }
        }
        return false;
    }

    // Handler methods
    private async Task HandleRegisterUser(Request request, Response response)
    {
        var credentials = JsonSerializer.Deserialize<UserCredentials>(request.Body);
        
        if (credentials == null || string.IsNullOrWhiteSpace(credentials.Username) || string.IsNullOrWhiteSpace(credentials.Password))
        {
            response.StatusCode = 400;
            response.ReasonPhrase = "Bad Request";
            response.Body = "Invalid login data. Username and password are required.";
            return;
        }

        var result = _userRouteHandler.RegisterUser(credentials);

        if (result.Success)
        {
            response.StatusCode = 200;
            response.ReasonPhrase = "OK";
            response.Body = "Registration successful.";
        }
        else
        {
            response.StatusCode = 400;
            response.ReasonPhrase = "Bad Request";
            response.Body = "Invalid login data. Username and password are required.";
        }
    }

    private async Task HandleLoginUser(Request request, Response response)
    {
        var credentials = JsonSerializer.Deserialize<UserCredentials>(request.Body);

        if (credentials == null || string.IsNullOrWhiteSpace(credentials.Username) || string.IsNullOrWhiteSpace(credentials.Password))
        {
            response.StatusCode = 400;
            response.ReasonPhrase = "Bad Request";
            response.Body = "Invalid login data. Username and password are required.";
            return;
        }

        var result = _userRouteHandler.LoginUser(credentials);

        if (result.Success)
        {
            response.StatusCode = 200;
            response.ReasonPhrase = "OK";
            response.Body = "Login successful.";
            // Optionally include authentication token in headers or response body
        }
        else
        {
            response.StatusCode = 401;
            response.ReasonPhrase = "Unauthorized";
            response.Body = result.ErrorMessage;
        }
    }

    
    private async Task HandleUpdateUser(Request request, Response response)
    {
        // Implementation
    }
}