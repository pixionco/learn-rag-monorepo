using Microsoft.Extensions.Options;
using Pixion.LearnRag.Core.Clients;
using Pixion.LearnRag.Core.Models;
using Pixion.LearnRag.Infrastructure.Configs;

namespace Pixion.LearnRag.Infrastructure.ClientMocks;

public class AzureOpenAiEmbeddingClientFailingMock(IOptions<PostgresVectorRepositoryConfig> config) : IEmbeddingClient
{
    public async Task<EmbeddingGenerationResult> GenerateEmbeddingAsync(string text)
    {
        var isSuccess = Convert.ToBoolean(new Random().Next(0, 2));
        if (!isSuccess) return new EmbeddingGenerationErrorResult(new Exception("Embedding failed!"), null);

        var embedding = new float[config.Value.VectorSize];
        var rand = new Random();

        for (var i = 0; i < embedding.Length; i++) embedding[i] = (float)rand.NextDouble();

        return await Task.FromResult(new EmbeddingGenerationSuccessResult(new ReadOnlyMemory<float>(embedding)));
    }
}