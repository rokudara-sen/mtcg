using MTCG._01_Presentation_Layer.Interfaces;
using MTCG._01_Presentation_Layer.Models.Http;

namespace MTCG._02_Business_Logic_Layer.Services
{
    public class Router
    {
        private readonly List<IEndpoint> _endpoints;

        public Router(IEnumerable<IEndpoint> endpoints)
        {
            _endpoints = endpoints.ToList() ?? throw new ArgumentNullException(nameof(endpoints));
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
}