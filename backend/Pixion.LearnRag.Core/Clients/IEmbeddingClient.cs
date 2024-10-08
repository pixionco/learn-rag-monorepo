using Pixion.LearnRag.Core.Models;

namespace Pixion.LearnRag.Core.Clients;

public interface IEmbeddingClient
{
    public Task<EmbeddingGenerationResult> GenerateEmbeddingAsync(string text);
}