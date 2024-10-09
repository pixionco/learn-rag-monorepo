using Pixion.LearnRag.Core.Models;

namespace Pixion.LearnRag.Core.Clients;

public interface IEmbeddingClient
{
    public Task<Optional<ReadOnlyMemory<float>>> GenerateEmbeddingAsync(string text);
    public Task<Optional<IList<ReadOnlyMemory<float>>>> GenerateEmbeddingsAsync(IList<string> text);
}