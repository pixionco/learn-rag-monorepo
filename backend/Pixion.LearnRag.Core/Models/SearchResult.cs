using System.Text.Json;
using Pixion.LearnRag.Core.Entities;

namespace Pixion.LearnRag.Core.Models;

public record SearchResult(string Text, double? Relevance, string MetadataString)
{
    private object? _cachedMetadata;
    public string Text { init; get; }
    public double? Relevance { init; get; }
    public string MetadataString { init; get; }

    public T Metadata<T>() where T : BasicMetadata
    {
        if (_cachedMetadata is not T) _cachedMetadata = JsonSerializer.Deserialize<T>(MetadataString);
        if (_cachedMetadata is null) throw new Exception("Invalid Metadata Deserialization");

        return (T)_cachedMetadata;
    }
}