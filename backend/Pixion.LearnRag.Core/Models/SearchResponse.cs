namespace Pixion.LearnRag.Core.Models;

public record SearchResponse(
    IEnumerable<SearchResult> SearchResults,
    int InputTokenCount,
    long ElapsedMilliseconds
);