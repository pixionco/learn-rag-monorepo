using Pixion.LearnRag.Core.Clients;
using Pixion.LearnRag.Core.Entities;
using Pixion.LearnRag.Core.Entities.EmbeddingOptions;
using Pixion.LearnRag.Core.Entities.Metadata;
using Pixion.LearnRag.Core.Enums;
using Pixion.LearnRag.Core.Models;
using Pixion.LearnRag.Core.Repositories;
using Pixion.LearnRag.Core.Services;
using Pixion.LearnRag.Core.Services.RetrievalStrategy;

namespace Pixion.LearnRag.UseCases.Services.RetrievalStrategy;

public class BasicStrategyService(
    IBasicStrategyRepository basicStrategyRepository,
    IEmbeddingClient embeddingClient,
    IChunkingService chunkingService
) : IBasicStrategyService
{
    public async Task<IEnumerable<SearchResult>> SearchAsync(
        ChunkSize chunkSize,
        ChunkOverlap chunkOverlap,
        string query,
        int limit
    )
    {
        var embeddingResult = await embeddingClient.GenerateEmbeddingAsync(query);
        if (embeddingResult is not EmbeddingGenerationSuccessResult successResult)
            throw (embeddingResult as EmbeddingGenerationErrorResult)!.Exception;

        var results = await basicStrategyRepository.SearchAsync(successResult.Embedding, limit);

        return results.OrderBy(x => x.Metadata<BasicMetadata>().Index);
    }

    public Task EmbedAsync(Guid documentId, BasicEmbeddingOptions embeddingOptions)
    {
        // get document from some storage?
        var contents = "adasdasdad";

        var chunkStrings =
            chunkingService.ChunkText(contents, embeddingOptions.ChunkSize, embeddingOptions.ChunkOverlap);
        var chunkEmbeddings = embeddingClient.GenerateEmbeddingAsync(chunkStrings)
        var chunks = chunkStrings.Select(text => new EmbeddingRecord(Guid.NewGuid(), text,))
    }
}