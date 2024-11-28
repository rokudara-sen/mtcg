using MCTG._01_Presentation_Layer.Models;
using MCTG._01_Presentation_Layer.Models.Http;

namespace MCTG._01_Presentation_Layer.Interfaces;

public interface IEndpoint
{
    Task HandleRequest(Request request, Response response);
    bool CanHandle(Request request);
}