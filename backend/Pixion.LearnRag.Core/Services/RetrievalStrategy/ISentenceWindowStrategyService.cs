using Pixion.LearnRag.Core.Enums;
using Pixion.LearnRag.Core.Models;

namespace Pixion.LearnRag.Core.Services.RetrievalStrategy;

public interface ISentenceWindowStrategyService
{
    Task<IEnumerable<SearchResult>> SearchAsync(
        ChunkSize chunkSize,
        string query,
        int range,
        int limit
    );
}