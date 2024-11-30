using MTCG._01_Presentation_Layer.Models.Http;

namespace MTCG._01_Presentation_Layer.Models;

public class Route
{
    public string HttpMethod { get; }
    public string PathPattern { get; }
    public Func<Request, Response, Task> Handler { get; }

    public Route(string httpMethod, string pathPattern, Func<Request, Response, Task> handler)
    {
        HttpMethod = httpMethod;
        PathPattern = pathPattern;
        Handler = handler;
    }
}