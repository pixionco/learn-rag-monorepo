using Pixion.LearnRag.Core.Clients;
using Pixion.LearnRag.Core.Entities.Metadata;
using Pixion.LearnRag.Core.Enums;
using Pixion.LearnRag.Core.Models;
using Pixion.LearnRag.Core.Repositories;
using Pixion.LearnRag.Core.Services.RetrievalStrategy;

namespace Pixion.LearnRag.UseCases.Services.RetrievalStrategy;

public class HypotheticalQuestionStrategyService(
    IHypotheticalQuestionStrategyRepository hypotheticalQuestionStrategyRepository,
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
        var embeddingResult = await embeddingClient.GenerateEmbeddingAsync(query);
        if (embeddingResult is not EmbeddingGenerationSuccessResult successResult)
            throw (embeddingResult as EmbeddingGenerationErrorResult)!.Exception;

        var results = await hypotheticalQuestionStrategyRepository.SearchAsync(successResult.Embedding, limit);

        return results.OrderBy(x => x.Metadata<HypotheticalQuestionMetadata>().Index);
    }
}