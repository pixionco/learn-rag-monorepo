using Pixion.LearnRag.Core.Models;

namespace Pixion.LearnRag.Core.Repositories;

public interface IAutoMergingStrategyRepository : IStrategyRepository
{
    public Task<IEnumerable<SearchResult>> GetParentsAsync(
        Guid documentId,
        IEnumerable<int> parentIndexes,
        CancellationToken cancellationToken = default
    );
}