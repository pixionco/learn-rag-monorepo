using Pixion.LearnRag.Core.Clients;
using Pixion.LearnRag.Core.Enums;
using Pixion.LearnRag.Core.Models;
using Pixion.LearnRag.Core.Repositories;
using Pixion.LearnRag.Core.Services.RetrievalStrategy;

namespace Pixion.LearnRag.UseCases.Services.RetrievalStrategy;

public class HypotheticalQuestionStrategyService(
    IEmbeddingRecordRepository embeddingRecordRepository,
    IEmbeddingClient embeddingClient
) : IHypotheticalQuestionStrategyService
{
    public async Task<IEnumerable<SearchResult>> SearchAsync(
        ChunkSize chunkSize,
        ChunkOverlap chunkOverlap,
        string query,
        int limit
    )
    {
        throw new Exception("Not implemented");
        /*var operations = string.Join(",", this.GetOperations(chunkSize, chunkOverlap));

        var operationPathId = await operationPathRepository.GetOperationPathId(operations);
        if (operationPathId == null)
          throw new Exception($"Operation path with operations {operationPathId} doesn't exist!");

        var embeddingResult = await embeddingClient.GenerateEmbeddingAsync(query);
        if (embeddingResult is not EmbeddingGenerationSuccessResult successResult)
          throw (embeddingResult as EmbeddingGenerationErrorResult)!.Exception;
        var results = await embeddingRecordRepository.SearchAsync(successResult.Embedding, operationPathId.Value, limit);

        return results.OrderBy(x => x.Metadata.Index);*/
    }
}