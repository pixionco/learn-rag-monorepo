using Pixion.LearnRag.Core.Enums;
using Pixion.LearnRag.Core.Models;
using Pixion.LearnRag.Core.Repositories;
using Pixion.LearnRag.Core.Services.RetrievalStrategy;

namespace Pixion.LearnRag.UseCases.Services.RetrievalStrategy;

public class HierarchicalStrategyService(
    IEmbeddingRecordRepository embeddingRecordRepository
) : IHierarchicalStrategyService
{
    public async Task<IEnumerable<SearchResult>> SearchAsync(
        ChunkSize chunkSize,
        ChunkOverlap chunkOverlap,
        ChunkSize childChunkSize,
        ChunkOverlap childChunkOverlap,
        string query,
        int limit,
        int childLimit
    )
    {
        throw new Exception("Not implemented");
        /*var jaggedOperations = this
          .GetOperations(
            chunkSize,
            chunkOverlap,
            childChunkSize,
            childChunkOverlap
          )
          .ToList();
        var parentOperations = string.Join(",", jaggedOperations[0]);
        var childOperations = string.Join(",", jaggedOperations[1]);

        var parentOperationPathId = await operationPathRepository.GetOperationPathId(parentOperations);
        if (parentOperationPathId == null)
          throw new Exception($"Operation path with operations {parentOperationPathId} doesn't exist!");

        var childOperationPathId = await operationPathRepository.GetOperationPathId(childOperations);
        if (childOperationPathId == null)
          throw new Exception($"Operation path with operations {childOperations} doesn't exist!");

        var embeddingResult = await embeddingClient.GenerateEmbeddingAsync(query);
        if (embeddingResult is not EmbeddingGenerationSuccessResult successResult)
          throw (embeddingResult as EmbeddingGenerationErrorResult)!.Exception;
        var parentResults =
          (await embeddingRecordRepository.SearchAsync(successResult.Embedding, parentOperationPathId.Value, limit))
          .ToList();

        var results = new List<SearchResult>();

        foreach (var parentResult in parentResults)
        {
          var childResults = await embeddingRecordRepository.SearchBySummaryAsync(
            successResult.Embedding,
            childOperationPathId.Value,
            parentResult.Metadata.DocumentId,
            parentResult.Metadata.Index,
            childLimit
          );
          results.AddRange(childResults);
        }

        return results
          .OrderBy(x => x.Metadata.ParentIndex ?? int.MaxValue)
          .ThenBy(x => x.Metadata.Index);*/
    }
}