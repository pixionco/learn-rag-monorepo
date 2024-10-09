using Pixion.LearnRag.Core.Clients;
using Pixion.LearnRag.Core.Entities;
using Pixion.LearnRag.Core.Enums;
using Pixion.LearnRag.Core.Models;
using Pixion.LearnRag.Core.Repositories;
using Pixion.LearnRag.Core.Services.RetrievalStrategy;

namespace Pixion.LearnRag.UseCases.Services.RetrievalStrategy;

public class BasicStrategyService(
    IBasicStrategyRepository basicStrategyRepository,
    IEmbeddingClient embeddingClient
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
}