using System.Net;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using Microsoft.SemanticKernel.Embeddings;
using Pixion.LearnRag.Core.Clients;
using Pixion.LearnRag.Core.Models;
using Pixion.LearnRag.Infrastructure.Configs;
using Polly;

namespace Pixion.LearnRag.Infrastructure.Clients;

#pragma warning disable SKEXP0001
#pragma warning disable SKEXP0010
#pragma warning disable SKEXP0011

public class AzureOpenAiEmbeddingClient(IOptions<AzureOpenAiEmbeddingConfig> config) : IEmbeddingClient
{
    private readonly AzureOpenAITextEmbeddingGenerationService _embeddingService = new(
        config.Value.DeploymentName,
        config.Value.Endpoint,
        config.Value.ApiKey
    );

    public async Task<EmbeddingGenerationResult> GenerateEmbeddingAsync(string text)
    {
        return await Policy
            .HandleResult<EmbeddingGenerationResult>(
                result => result is EmbeddingGenerationErrorResult { StatusCode: HttpStatusCode.TooManyRequests }
            )
            .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)))
            .ExecuteAsync(async () => await GenerateEmbeddingHelperAsync(text));
    }

    private async Task<EmbeddingGenerationResult> GenerateEmbeddingHelperAsync(string text)
    {
        try
        {
            var embedding = await _embeddingService.GenerateEmbeddingAsync(text);

            return new EmbeddingGenerationSuccessResult(embedding);
        }
        catch (HttpOperationException ex)
        {
            return new EmbeddingGenerationErrorResult(ex, ex.StatusCode);
        }
        catch (Exception ex)
        {
            return new EmbeddingGenerationErrorResult(ex, null);
        }
    }
}