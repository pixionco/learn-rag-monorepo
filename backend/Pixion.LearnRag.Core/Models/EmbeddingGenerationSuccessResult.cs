namespace Pixion.LearnRag.Core.Models;

public record EmbeddingGenerationSuccessResult(ReadOnlyMemory<float> Embedding) : EmbeddingGenerationResult;