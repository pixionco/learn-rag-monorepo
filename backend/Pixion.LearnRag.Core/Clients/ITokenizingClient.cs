using Pixion.LearnRag.Core.Enums;

namespace Pixion.LearnRag.Core.Clients;

public interface ITokenizingClient
{
    int GetTokenCount(string text, AiModel aiModel);

    int GetLargeLanguageModelTokenCount(string text);

    int GetEmbeddingModelTokenCount(string text);
}