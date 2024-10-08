namespace Pixion.LearnRag.Core.Models;

public abstract record EmbeddingGenerationResult
{
    public bool IsSuccess => this is EmbeddingGenerationSuccessResult;
}