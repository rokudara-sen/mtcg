namespace MTCG._00_Server;

internal class StreamTracer
{
    private StreamWriter _streamWriter;

    public StreamTracer(StreamWriter streamWriter)
    {
        _streamWriter = streamWriter;
    }

    internal async Task WriteLineAsync(string v)
    {
        Console.WriteLine(v);
        await _streamWriter.WriteLineAsync(v);
    }

    internal async Task WriteLineAsync()
    {
        Console.WriteLine();
        await _streamWriter.WriteLineAsync();
    }
}