using Pixion.LearnRag.Core.Clients;
using Pixion.LearnRag.Core.Entities;
using Pixion.LearnRag.Core.Enums;
using Pixion.LearnRag.Core.Models;
using Pixion.LearnRag.Core.Repositories;
using Pixion.LearnRag.Core.Services.RetrievalStrategy;

namespace Pixion.LearnRag.UseCases.Services.RetrievalStrategy;

public class AutoMergingStrategyService(
    IAutoMergingStrategyRepository autoMergingStrategyRepository,
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
        var embeddingResult = await embeddingClient.GenerateEmbeddingAsync(query);
        if (embeddingResult is not EmbeddingGenerationSuccessResult successResult)
            throw (embeddingResult as EmbeddingGenerationErrorResult)!.Exception;

        var childResults =
            (await autoMergingStrategyRepository.SearchLeafChunksAsync(successResult.Embedding, limit))
            .ToList();

        if (childResults.Count == 0) return childResults;

        var n = (int)Math.Ceiling(
            chunkSize.Value / Convert.ToDouble(childChunkSize.Value) * childParentPrevalenceFactor);
        var allSearchResults = new List<SearchResult>();

        foreach (var documentIdChildResults in childResults.GroupBy(
                     el => el.Metadata<HierarchicalMetadata>().DocumentId))
        {
            var childResultsToNotMerge = documentIdChildResults
                .GroupBy(el => el.Metadata<HierarchicalMetadata>().ParentIndex)
                .Where(group => group.Count() < n)
                .SelectMany(group => group)
                .OrderBy(childResult => childResult.Metadata<HierarchicalMetadata>().Index);

            allSearchResults.AddRange(childResultsToNotMerge);

            var parentIndexes = documentIdChildResults
                .GroupBy(el => el.Metadata<HierarchicalMetadata>().ParentIndex)
                .Where(group => group.Count() >= n)
                .Select(group => group.Key!.Value);

            var parentSearchResults = await autoMergingStrategyRepository.GetParentChunksAsync(
                documentIdChildResults.Key,
                parentIndexes
            );
            allSearchResults.AddRange(parentSearchResults);
        }

        return allSearchResults
            .Select(
                x => new
                {
                    SearchResult = x,
                    EffectiveParentIndex = x.Metadata<HierarchicalMetadata>().ParentIndex ??
                                           x.Metadata<HierarchicalMetadata>().Index
                }
            )
            .OrderBy(x => x.EffectiveParentIndex)
            .ThenBy(x => x.SearchResult.Metadata<HierarchicalMetadata>().Index)
            .Select(x => x.SearchResult);
    }
}