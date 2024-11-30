namespace MTCG._00_Server;

internal class StreamTracer
{
    private StreamWriter streamWriter;

    public StreamTracer(StreamWriter streamWriter)
    {
        this.streamWriter = streamWriter;
    }

    internal async Task WriteLineAsync(string v)
    {
        Console.WriteLine(v);
        await streamWriter.WriteLineAsync(v);
    }

    internal async Task WriteLineAsync()
    {
        Console.WriteLine();
        await streamWriter.WriteLineAsync();
    }
}