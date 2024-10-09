using Pixion.LearnRag.Core.Clients;
using Pixion.LearnRag.Core.Entities;
using Pixion.LearnRag.Core.Enums;
using Pixion.LearnRag.Core.Models;
using Pixion.LearnRag.Core.Repositories;
using Pixion.LearnRag.Core.Services.RetrievalStrategy;

namespace Pixion.LearnRag.UseCases.Services.RetrievalStrategy;

public class HierarchicalStrategyService(
    IHierarchicalStrategyRepository hierarchicalStrategyRepository,
    IEmbeddingClient embeddingClient
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
        var embeddingResult = await embeddingClient.GenerateEmbeddingAsync(query);
        if (embeddingResult is not EmbeddingGenerationSuccessResult successResult)
            throw (embeddingResult as EmbeddingGenerationErrorResult)!.Exception;

        var parentResults =
            (await hierarchicalStrategyRepository.SearchFirstLevelAsync(successResult.Embedding, limit))
            .ToList();

        var results = new List<SearchResult>();

        foreach (var parentResult in parentResults)
        {
            var childResults = await hierarchicalStrategyRepository.SearchByParentAsync(
                successResult.Embedding,
                parentResult.Metadata<HierarchicalMetadata>().DocumentId,
                parentResult.Metadata<HierarchicalMetadata>().Index,
                childLimit
            );
            results.AddRange(childResults);
        }

        return results
            .OrderBy(x => x.Metadata<HierarchicalMetadata>().ParentIndex ?? int.MaxValue)
            .ThenBy(x => x.Metadata<HierarchicalMetadata>().Index);
    }
}