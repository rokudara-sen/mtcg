namespace MCTG._01_Presentation_Layer.Models.Http;

public class Request
{
    public string Method { get; set; } = "";
    public string Path { get; set; } = "";
    public string HttpVersion { get; set; } = "";
    public Dictionary<string, string> Headers { get; set; } = new();
    public string Body { get; set; } = "";
    public Dictionary<string, string> RouteParameters { get; set; }
}