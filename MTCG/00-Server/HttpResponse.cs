using MTCG._01_Presentation_Layer.Models.Http;

namespace MTCG._00_Server;

public class HttpResponse
{
    public async Task SendResponseAsync(StreamWriter writer, Response response)
    {
        var writerAlsoToConsole = new StreamTracer(writer);

        await writerAlsoToConsole.WriteLineAsync($"{response.HttpVersion} {response.StatusCode} {response.ReasonPhrase}");

        // Ensure Content-Length header is set
        if (!response.Headers.ContainsKey("Content-Length"))
        {
            response.Headers["Content-Length"] = response.Body.Length.ToString();
        }

        // Write headers
        foreach (var header in response.Headers)
        {
            await writerAlsoToConsole.WriteLineAsync($"{header.Key}: {header.Value}");
        }

        // Empty line to indicate end of headers
        await writerAlsoToConsole.WriteLineAsync();

        // Write the body
        if (!string.IsNullOrEmpty(response.Body))
        {
            await writerAlsoToConsole.WriteLineAsync(response.Body);
        }

        Console.WriteLine("========================================");
    }
}