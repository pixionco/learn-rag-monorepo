using Pixion.LearnRag.Core.Clients;
using Pixion.LearnRag.Core.Enums;
using Pixion.LearnRag.Infrastructure.Interfaces;

namespace Pixion.LearnRag.Infrastructure.Clients;

public class TokenizingClient(ITokenizerProvider tokenizerProvider) : ITokenizingClient
{
    private readonly IReadOnlyCollection<string> _specialTokens = new List<string>();

    public int GetTokenCount(string text, AiModel aiModel)
    {
        var tokenizer = tokenizerProvider.GetTokenizer(aiModel.Value);

        return tokenizer.Encode(text, _specialTokens).Count;
    }

    public int GetLargeLanguageModelTokenCount(string text)
    {
        var tokenizer = tokenizerProvider.GetTokenizer(AiModel.Gpt35Turbo.Value);

        return tokenizer.Encode(text, _specialTokens).Count;
    }

    public int GetEmbeddingModelTokenCount(string text)
    {
        var tokenizer = tokenizerProvider.GetTokenizer(AiModel.TextEmbedding3Large.Value);

        return tokenizer.Encode(text, _specialTokens).Count;
    }
}