using MCTG._00_Server;

namespace MCTG._02_Business_Logic_Layer.Interfaces;

public interface IRouteHandler
{
    void HandleRequest(HttpRequest request, HttpResponse response);
}