using System.Collections.Concurrent;
using System.Text.Json;
using MTCG._01_Presentation_Layer.Interfaces;
using MTCG._01_Presentation_Layer.Models;
using MTCG._01_Presentation_Layer.Models.Http;
using MTCG._02_Business_Logic_Layer.RouteHandlers;
using MTCG._03_Data_Access_Layer.DataContext;
using MTCG._06_Domain.Entities;
using MTCG._06_Domain.ValueObjects;

namespace MTCG._01_Presentation_Layer.Endpoints;

public class PackageEndpoint : IEndpoint
{
    private readonly List<Route> _routes;
    private readonly PackageRouteHandler _packageRouteHandler;
    private readonly UserRouteHandler _userRouteHandler;
    public PackageEndpoint()
    {
        _packageRouteHandler = new PackageRouteHandler();
        _userRouteHandler = new UserRouteHandler();
        _routes = [
            new Route("POST", "/packages", HandleCreatePackage)
        ];
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
        if (!request.Headers.TryGetValue("Authorization", out var authHeader) || string.IsNullOrWhiteSpace(authHeader))
        {
            response.StatusCode = 400;
            response.ReasonPhrase = "Bad Request: Missing Authorization Header";
            return;
        }
        
        var user = await Task.Run(() => _userRouteHandler.GetUserByAuthToken(authHeader.Replace("Bearer ", "")));
        
        if (user == null || !_packageRouteHandler.CheckIfAdmin(user))
        {
            response.StatusCode = 401;
            response.ReasonPhrase = "Unauthorized";
            return;
        }

        List<Card> cardsList;
        try
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            
            cardsList = JsonSerializer.Deserialize<List<Card>>(request.Body, options);
        }
        catch (JsonException ex)
        {
            response.StatusCode = 400;
            response.ReasonPhrase = "Bad Request: Invalid JSON Format";
            return;
        }
        
        if (cardsList == null || cardsList.Count == 0)
        {
            response.StatusCode = 400;
            response.ReasonPhrase = "Bad Request: No Cards Provided";
            return;
        }

        var package = new Package(cardsList);
        
        var result = await Task.Run(() => _packageRouteHandler.CreatePackage(user, package));

        if (result.Success)
        {
            response.StatusCode = 201;
            response.ReasonPhrase = "Created";
        }
        else
        {
            response.StatusCode = 400;
            response.ReasonPhrase = "Something went wrong";
        }
    }
}