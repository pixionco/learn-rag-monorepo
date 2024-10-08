using Pixion.LearnRag.Core.Entities;
using Pixion.LearnRag.Core.Models;

namespace Pixion.LearnRag.Core.Repositories;

public interface IEmbeddingRecordRepository
{
    Task InsertOneAsync(EmbeddingRecord embeddingRecord, CancellationToken cancellationToken = default);

    Task InsertManyAsync(IEnumerable<EmbeddingRecord> embeddingRecords, CancellationToken cancellationToken = default);

    Task<IEnumerable<SearchResult>> SearchAsync(
        ReadOnlyMemory<float> queryEmbedding,
        int operationPathId,
        int limit,
        CancellationToken cancellationToken = default
    );

    Task<IEnumerable<SearchResult>> SearchBySummaryAsync(
        ReadOnlyMemory<float> queryEmbedding,
        int operationPathId,
        Guid documentId,
        int parentIndex,
        int limit,
        CancellationToken cancellationToken = default
    );

    Task<IEnumerable<SearchResult>> GetParentChunksAsync(
        IEnumerable<int> parentIndexes,
        Guid documentId,
        int operationPathId,
        CancellationToken cancellationToken = default
    );

    Task<IEnumerable<SearchResult>> GetNearbyChunksAsync(
        Guid documentId,
        int index,
        int range,
        int operationPathId,
        CancellationToken cancellationToken = default
    );

    /*
    Task<IEnumerable<string>> GetBatchOfChunksAsync(
        int chunkSize,
        int chunkOverlap,
        int batchSize,
        int offset,
        CancellationToken cancellationToken = default
    );

    Task<IEnumerable<string>> GetBatchOfChildChunksAsync(
        int chunkSize,
        int chunkOverlap,
        int childChunkSize,
        int childChunkOverlap,
        int batchSize,
        int offset,
        CancellationToken cancellationToken = default
    );

    Task<IEnumerable<string>> GetBatchOfQuestionsAsync(
        int chunkSize,
        int chunkOverlap,
        int batchSize,
        int offset,
        CancellationToken cancellationToken = default
    );

    Task<IEnumerable<SummaryDetails>> GetBatchOfSummariesAsync(
        int chunkSize,
        int chunkOverlap,
        int batchSize,
        int offset,
        CancellationToken cancellationToken = default
    );*/

    Task<int> DeleteByOperationTreeIdAsync(Guid operationTreeId, CancellationToken cancellationToken = default);
}