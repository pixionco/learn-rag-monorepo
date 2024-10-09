using Pixion.LearnRag.Core.Entities;
using Pixion.LearnRag.Core.Models;

namespace Pixion.LearnRag.Core.Repositories;

public interface IStrategyRepository
{
    Task InsertAsync(EmbeddingRecord embeddingRecord, CancellationToken cancellationToken = default);
    Task BatchInsertAsync(IEnumerable<EmbeddingRecord> embeddingRecords, CancellationToken cancellationToken = default);

    Task<IEnumerable<SearchResult>> SearchAsync(
        ReadOnlyMemory<float> queryEmbedding,
        int limit,
        CancellationToken cancellationToken = default
    );
}