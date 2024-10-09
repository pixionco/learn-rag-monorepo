using Pixion.LearnRag.Core.Models;

namespace Pixion.LearnRag.Core.Repositories;

public interface IHierarchicalStrategyRepository : IStrategyRepository
{
    public Task<IEnumerable<SearchResult>> SearchByParentAsync(
        ReadOnlyMemory<float> queryEmbedding,
        Guid documentId,
        int parentIndex,
        int limit,
        CancellationToken cancellationToken = default
    );

    public Task<IEnumerable<SearchResult>> SearchRootChunksAsync(
        ReadOnlyMemory<float> queryEmbedding,
        int limit,
        CancellationToken cancellationToken = default
    );
}