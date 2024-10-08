using Pixion.LearnRag.Core.Clients;
using Pixion.LearnRag.Core.Enums;
using Pixion.LearnRag.Core.Models;
using Pixion.LearnRag.Core.Repositories;
using Pixion.LearnRag.Core.Services.RetrievalStrategy;

namespace Pixion.LearnRag.UseCases.Services.RetrievalStrategy;

public class AutoMergingStrategyService(
    IEmbeddingRecordRepository embeddingRecordRepository,
    IEmbeddingClient embeddingClient
) : IAutoMergingStrategyService
{
    public async Task<IEnumerable<SearchResult>> SearchAsync(
        ChunkSize chunkSize,
        ChunkOverlap chunkOverlap,
        ChunkSize childChunkSize,
        ChunkOverlap childChunkOverlap,
        double childParentPrevalenceFactor,
        string query,
        int limit
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

        var childResults = (await embeddingRecordRepository.SearchAsync(successResult.Embedding, childOperationPathId.Value, limit))
          .ToList();

        if (childResults.Count == 0) return Enumerable.Empty<SearchResult>();

        var n = (int)Math.Ceiling(chunkSize.Value / Convert.ToDouble(childChunkSize.Value) * childParentPrevalenceFactor);
        var allSearchResults = new List<SearchResult>();

        foreach (var documentIdChildResults in childResults.GroupBy(el => el.Metadata.DocumentId))
        {
          var childResultsToNotMerge = documentIdChildResults
            .GroupBy(el => el.Metadata.ParentIndex)
            .Where(group => group.Count() < n)
            .SelectMany(group => group)
            .OrderBy(childResult => childResult.Metadata.Index);

          allSearchResults.AddRange(childResultsToNotMerge);

          var parentIndexes = documentIdChildResults
            .GroupBy(el => el.Metadata.ParentIndex)
            .Where(group => group.Count() >= n)
            .Select(group => group.Key!.Value);

          var parentSearchResults = await embeddingRecordRepository.GetParentChunksAsync(
            parentIndexes,
            documentIdChildResults.Key,
            parentOperationPathId.Value
          );
          allSearchResults.AddRange(parentSearchResults);
        }

        return allSearchResults
          .Select(
            x => new
            {
              SearchResult = x,
              EffectiveParentIndex = x.Metadata.ParentIndex ?? x.Metadata.Index
            }
          )
          .OrderBy(x => x.EffectiveParentIndex)
          .ThenBy(x => x.SearchResult.Metadata.Index)
          .Select(x => x.SearchResult);*/
    }
}