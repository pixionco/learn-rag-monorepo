namespace Pixion.LearnRag.Core.Entities.EmbeddingOptions;

public record AutoMergingEmbeddingOptions(
    int ChunkSize,
    int ChunkOverlap,
    int ChildChunkSize,
    int ChildChunkOverlap);