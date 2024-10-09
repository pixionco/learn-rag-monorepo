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

    public async Task<Optional<IList<ReadOnlyMemory<float>>>> GenerateEmbeddingsAsync(IList<string> texts)
    {
        return await Policy
            .HandleResult<Optional<IList<ReadOnlyMemory<float>>>>(
                result => result.Exception is HttpOperationException { StatusCode: HttpStatusCode.TooManyRequests }
            )
            .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)))
            .ExecuteAsync(async () => await GenerateEmbeddingsHelperAsync(texts));
    }

    public async Task<Optional<ReadOnlyMemory<float>>> GenerateEmbeddingAsync(string text)
    {
        return await Policy
            .HandleResult<Optional<ReadOnlyMemory<float>>>(
                result => result.Exception is HttpOperationException { StatusCode: HttpStatusCode.TooManyRequests }
            )
            .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)))
            .ExecuteAsync(async () => await GenerateEmbeddingHelperAsync(text));
    }


    // TODO: add error control, but in a more normal way
    private async Task<Optional<IList<ReadOnlyMemory<float>>>> GenerateEmbeddingsHelperAsync(IList<string> texts)
    {
        try
        {
            var embeddings = await _embeddingService.GenerateEmbeddingsAsync(texts);

            return new Optional<IList<ReadOnlyMemory<float>>>(embeddings);
        }
        catch (HttpOperationException ex)
        {
            return new Optional<IList<ReadOnlyMemory<float>>>(ex);
        }
        catch (Exception ex)
        {
            return new Optional<IList<ReadOnlyMemory<float>>>(ex);
        }
    }

    private async Task<Optional<ReadOnlyMemory<float>>> GenerateEmbeddingHelperAsync(string text)
    {
        try
        {
            var embedding = await _embeddingService.GenerateEmbeddingAsync(text);

            return new Optional<ReadOnlyMemory<float>>(embedding);
        }
        catch (HttpOperationException ex)
        {
            return new Optional<ReadOnlyMemory<float>>(ex);
        }
        catch (Exception ex)
        {
            return new Optional<ReadOnlyMemory<float>>(ex);
        }
    }
}