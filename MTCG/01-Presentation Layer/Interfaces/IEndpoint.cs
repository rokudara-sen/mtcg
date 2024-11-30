using MTCG._01_Presentation_Layer.Models;
using MTCG._01_Presentation_Layer.Models.Http;

namespace MTCG._01_Presentation_Layer.Interfaces;

public interface IEndpoint
{
    Task HandleRequest(Request request, Response response);
    bool CanHandle(Request request);
}