using Pixion.LearnRag.Core.Entities.EmbeddingOptions;
using Pixion.LearnRag.Core.Enums;
using Pixion.LearnRag.Core.Models;

namespace Pixion.LearnRag.Core.Services.RetrievalStrategy;

public interface IBasicStrategyService
{
    Task<IEnumerable<SearchResult>> SearchAsync(
        ChunkSize chunkSize,
        ChunkOverlap chunkOverlap,
        string query,
        int limit
    );

    Task EmbedAsync(
        Guid documentId,
        BasicEmbeddingOptions embeddingOptions
    );
}