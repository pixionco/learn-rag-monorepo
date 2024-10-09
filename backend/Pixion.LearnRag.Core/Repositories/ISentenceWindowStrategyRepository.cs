using Pixion.LearnRag.Core.Models;

namespace Pixion.LearnRag.Core.Repositories;

public interface ISentenceWindowStrategyRepository : IStrategyRepository
{
    Task<IEnumerable<SearchResult>> GetNearbyChunksAsync(
        Guid documentId,
        int index,
        int range,
        CancellationToken cancellationToken = default
    );
}