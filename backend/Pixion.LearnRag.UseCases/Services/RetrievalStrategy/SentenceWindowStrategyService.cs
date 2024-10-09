using System.Text.Json;
using Pixion.LearnRag.Core.Clients;
using Pixion.LearnRag.Core.Entities;
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
            var metadata = JsonSerializer.Deserialize<BasicMetadata>(result.MetadataString);
            if (metadata is null) throw new Exception("Metadata is missing");

            var nearby = (await sentenceWindowStrategyRepository.GetNearbyChunksAsync(
                metadata.DocumentId,
                metadata.Index,
                range
            )).ToList();
            var before = nearby
                .Where(x => x.Metadata<BasicMetadata>().Index < result.Metadata<BasicMetadata>().Index)
                .OrderBy(x => x.Metadata<BasicMetadata>().Index)
                .Select(x => x.Text)
                .ToList();
            var after = nearby
                .Where(x => x.Metadata<BasicMetadata>().Index > result.Metadata<BasicMetadata>().Index)
                .OrderBy(x => x.Metadata<BasicMetadata>().Index)
                .Select(x => x.Text)
                .ToList();

            items.Add(result with { Text = string.Join("", before) + result.Text + string.Join("", after) });
        }

        return items.OrderBy(x => x.Metadata<BasicMetadata>().Index);
    }
}