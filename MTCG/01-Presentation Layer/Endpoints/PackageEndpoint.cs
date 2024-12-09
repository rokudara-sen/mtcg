using System.Text.Json;
using MTCG._01_Presentation_Layer.Interfaces;
using MTCG._01_Presentation_Layer.Models;
using MTCG._01_Presentation_Layer.Models.Http;
using MTCG._02_Business_Logic_Layer.RouteHandlers;
using MTCG._03_Data_Access_Layer.Services;
using MTCG._06_Domain.Entities;

namespace MTCG._01_Presentation_Layer.Endpoints;

public class PackageEndpoint : IEndpoint
{
    private readonly List<Route> _routes;
    private readonly PackageRouteHandler _packageRouteHandler;
    private readonly UserRouteHandler _userRouteHandler;
    private readonly CardRouteHandler _cardRouteHandler;
    private readonly PackageAcquisitionService _packageAcquisitionService;

    public PackageEndpoint(PackageRouteHandler packageRouteHandler,
        UserRouteHandler userRouteHandler,
        CardRouteHandler cardRouteHandler,
        PackageAcquisitionService packageAcquisitionService)
    {
        _packageRouteHandler = packageRouteHandler ?? throw new ArgumentNullException(nameof(packageRouteHandler));
        _userRouteHandler = userRouteHandler ?? throw new ArgumentNullException(nameof(userRouteHandler));
        _cardRouteHandler = cardRouteHandler ?? throw new ArgumentNullException(nameof(cardRouteHandler));
        _packageAcquisitionService = packageAcquisitionService ?? throw new ArgumentNullException(nameof(packageAcquisitionService));
        _routes = new List<Route>
        {
            new Route("POST", "/packages", HandleCreatePackage),
            new Route("POST", "/transactions/packages", HandleAssignPackage)
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

    private Task HandleCreatePackage(Request request, Response response)
    {
        if (!request.Headers.TryGetValue("Authorization", out var authHeader) || string.IsNullOrWhiteSpace(authHeader))
        {
            response.StatusCode = 400;
            response.ReasonPhrase = "Bad Request: Missing Authorization Header";
            return Task.CompletedTask;
        }

        var user = _userRouteHandler.GetUserByAuthToken(authHeader.Replace("Bearer ", ""));

        if (user == null || !_userRouteHandler.IsValidUser(user))
        {
            response.StatusCode = 401;
            response.ReasonPhrase = "Unauthorized";
            return Task.CompletedTask;
        }

        if (user.Authorization != "admin-mtcgToken")
        {
            response.StatusCode = 403;
            response.ReasonPhrase = "Forbidden: User is not an admin";
            return Task.CompletedTask;
        }

        List<Card>? cardsList;
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
            response.Body = ex.Message;
            return Task.CompletedTask;
        }

        if (cardsList == null || cardsList.Count != 5)
        {
            response.StatusCode = 400;
            response.ReasonPhrase = "Bad Request: Exactly 5 cards must be provided";
            return Task.CompletedTask;
        }

        foreach (var card in cardsList)
        {
            var result = _cardRouteHandler.CreateCard(card);
            if (!result.Success)
            {
                response.StatusCode = 400;
                response.ReasonPhrase = "Bad Request: " + result.ErrorMessage;
                response.Body = result.ErrorMessage;
                return Task.CompletedTask;
            }
        }

        var package = new Package(cardsList);
        var packageResult = _packageRouteHandler.CreatePackage(user, package);

        if (packageResult.Success)
        {
            response.StatusCode = 201;
            response.ReasonPhrase = "Created";
        }
        else
        {
            response.StatusCode = 400;
            response.ReasonPhrase = "Bad Request";
            response.Body = packageResult.ErrorMessage;
        }

        return Task.CompletedTask;
    }
    
    private Task HandleAssignPackage(Request request, Response response)
    {
        if (!request.Headers.TryGetValue("Authorization", out var authHeader) || string.IsNullOrWhiteSpace(authHeader))
        {
            response.StatusCode = 400;
            response.ReasonPhrase = "Bad Request: Missing Authorization Header";
            return Task.CompletedTask;
        }

        var user = _userRouteHandler.GetUserByAuthToken(authHeader.Replace("Bearer ", ""));

        if (user == null)
        {
            response.StatusCode = 401;
            response.ReasonPhrase = "Invalid User";
            return Task.CompletedTask;
        }

        var result = _packageAcquisitionService.AcquirePackage(user);

        if (result.Success)
        {
            response.StatusCode = 201;
            response.ReasonPhrase = "OK";
        }
        else
        {
            response.StatusCode = 400;
            response.ReasonPhrase = result.ErrorMessage;
            response.Body = result.ErrorMessage;
        }

        return Task.CompletedTask;
    }
}
