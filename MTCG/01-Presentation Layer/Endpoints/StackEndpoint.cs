using MTCG._01_Presentation_Layer.Interfaces;
using MTCG._01_Presentation_Layer.Models.Http;

namespace MTCG._01_Presentation_Layer.Endpoints;

public class StackEndpoint : IEndpoint
{
    public Task HandleRequest(Request request, Response response)
    {
        throw new NotImplementedException();
    }

    public bool CanHandle(Request request)
    {
        throw new NotImplementedException();
    }
}