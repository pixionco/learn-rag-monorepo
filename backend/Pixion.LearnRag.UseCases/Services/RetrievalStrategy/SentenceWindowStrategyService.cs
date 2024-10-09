using System.Text.Json;
using Pixion.LearnRag.Core.Clients;
using Pixion.LearnRag.Core.Entities.Metadata;
using Pixion.LearnRag.Core.Enums;
using Pixion.LearnRag.Core.Models;
using Pixion.LearnRag.Core.Repositories;
using Pixion.LearnRag.Core.Services.RetrievalStrategy;

namespace Pixion.LearnRag.UseCases.Services.RetrievalStrategy;

public class SentenceWindowStrategyService(
    ISentenceWindowStrategyRepository sentenceWindowStrategyRepository,
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
        var embeddingResult = await embeddingClient.GenerateEmbeddingAsync(query);
        if (embeddingResult is not EmbeddingGenerationSuccessResult successResult)
            throw (embeddingResult as EmbeddingGenerationErrorResult)!.Exception;

        var results = await sentenceWindowStrategyRepository.SearchAsync(successResult.Embedding, limit);

        var items = new List<SearchResult>();
        foreach (var result in results)
        {
            var metadata = JsonSerializer.Deserialize<SentenceWindowMetadata>(result.MetadataString);
            if (metadata is null) throw new Exception("Metadata is missing");

            var nearby = (await sentenceWindowStrategyRepository.GetNearbyChunksAsync(
                metadata.DocumentId,
                metadata.Index,
                range
            )).ToList();
            var before = nearby
                .Where(x => x.Metadata<SentenceWindowMetadata>().Index <
                            result.Metadata<SentenceWindowMetadata>().Index)
                .OrderBy(x => x.Metadata<SentenceWindowMetadata>().Index)
                .Select(x => x.Text)
                .ToList();
            var after = nearby
                .Where(x => x.Metadata<SentenceWindowMetadata>().Index >
                            result.Metadata<SentenceWindowMetadata>().Index)
                .OrderBy(x => x.Metadata<SentenceWindowMetadata>().Index)
                .Select(x => x.Text)
                .ToList();

            items.Add(result with { Text = string.Join("", before) + result.Text + string.Join("", after) });
        }

        return items.OrderBy(x => x.Metadata<SentenceWindowMetadata>().Index);
    }
}