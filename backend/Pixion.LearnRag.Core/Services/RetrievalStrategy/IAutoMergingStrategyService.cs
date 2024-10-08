using Pixion.LearnRag.Core.Enums;
using Pixion.LearnRag.Core.Models;

namespace Pixion.LearnRag.Core.Services.RetrievalStrategy;

public interface IAutoMergingStrategyService
{
    Task<IEnumerable<SearchResult>> SearchAsync(
        ChunkSize chunkSize,
        ChunkOverlap chunkOverlap,
        ChunkSize childChunkSize,
        ChunkOverlap childChunkOverlap,
        double childParentPrevalenceFactor,
        string query,
        int limit
    );
}