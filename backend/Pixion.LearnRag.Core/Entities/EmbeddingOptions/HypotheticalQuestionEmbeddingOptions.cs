namespace Pixion.LearnRag.Core.Entities.EmbeddingOptions;

public record HypotheticalQuestionEmbeddingOptions(
    int ChunkSize,
    int ChunkOverlap,
    int NumOfQuestions);