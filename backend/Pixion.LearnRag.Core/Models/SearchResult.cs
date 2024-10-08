using Pixion.LearnRag.Core.Entities;

namespace Pixion.LearnRag.Core.Models;

public record SearchResult(
    string Text,
    double? Relevance,
    Metadata Metadata
);