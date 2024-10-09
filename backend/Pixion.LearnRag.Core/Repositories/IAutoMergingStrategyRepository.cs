using Pixion.LearnRag.Core.Models;

namespace Pixion.LearnRag.Core.Repositories;

public interface IAutoMergingStrategyRepository : IStrategyRepository
{
    public Task<IEnumerable<SearchResult>> GetParentChunksAsync(
        Guid documentId,
        IEnumerable<int> parentIndexes,
        CancellationToken cancellationToken = default
    );

    public Task<IEnumerable<SearchResult>> SearchLeafChunksAsync(
        ReadOnlyMemory<float> queryEmbedding,
        int limit,
        CancellationToken cancellationToken = default
    );
}