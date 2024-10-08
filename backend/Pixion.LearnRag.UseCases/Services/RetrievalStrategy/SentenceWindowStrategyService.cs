using Pixion.LearnRag.Core.Clients;
using Pixion.LearnRag.Core.Enums;
using Pixion.LearnRag.Core.Models;
using Pixion.LearnRag.Core.Repositories;
using Pixion.LearnRag.Core.Services.RetrievalStrategy;

namespace Pixion.LearnRag.UseCases.Services.RetrievalStrategy;

public class SentenceWindowStrategyService(
    IEmbeddingRecordRepository embeddingRecordRepository,
    IEmbeddingClient embeddingClient
) : ISentenceWindowStrategyService
{
    public async Task<IEnumerable<SearchResult>> SearchAsync(
        ChunkSize chunkSize,
        string query,
        int range,
        int limit
    )
    {
        throw new Exception("Not implemented");
        /*var operations = string.Join(",", this.GetOperations(chunkSize));

        var operationPathId = await operationPathRepository.GetOperationPathId(operations);
        if (operationPathId == null)
          throw new Exception($"Operation path with operations {operationPathId} doesn't exist!");

        var embeddingResult = await embeddingClient.GenerateEmbeddingAsync(query);
        if (embeddingResult is not EmbeddingGenerationSuccessResult successResult)
          throw (embeddingResult as EmbeddingGenerationErrorResult)!.Exception;
        var results = await embeddingRecordRepository.SearchAsync(successResult.Embedding, operationPathId.Value, limit);

        var items = new List<SearchResult>();

        foreach (var result in results)
        {
          var nearby = (await embeddingRecordRepository.GetNearbyChunksAsync(
            result.Metadata.DocumentId,
            result.Metadata.Index,
            range,
            operationPathId.Value
          )).ToList();
          var before = nearby
            .Where(x => x.Metadata.Index < result.Metadata.Index)
            .OrderBy(x => x.Metadata.Index)
            .Select(x => x.Text)
            .ToList();
          var after = nearby
            .Where(x => x.Metadata.Index > result.Metadata.Index)
            .OrderBy(x => x.Metadata.Index)
            .Select(x => x.Text)
            .ToList();

          items.Add(result with { Text = string.Join("", before) + result.Text + string.Join("", after) });
        }

        return items.OrderBy(x => x.Metadata.Index);*/
    }
}