using Pixion.LearnRag.Core.Enums;
using Pixion.LearnRag.Core.Models;

namespace Pixion.LearnRag.Core.Services.RetrievalStrategy;

public interface IHierarchicalStrategyService
{
    Task<IEnumerable<SearchResult>> SearchAsync(
        ChunkSize chunkSize,
        ChunkOverlap chunkOverlap,
        ChunkSize childChunkSize,
        ChunkOverlap childChunkOverlap,
        string query,
        int limit,
        int childLimit
    );
}