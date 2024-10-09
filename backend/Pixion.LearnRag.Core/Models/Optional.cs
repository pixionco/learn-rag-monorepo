namespace Pixion.LearnRag.Core.Models;

public class Optional<T>(bool ok, T? value, Exception? exception)
{
    public Optional(Exception? exception) : this(false, default, exception)
    {
    }

    public Optional(T value) : this(true, value, null)
    {
    }

    public bool Ok { init; get; } = ok;
    public T? Value { init; get; } = value;
    public Exception? Exception { get; } = exception;
}