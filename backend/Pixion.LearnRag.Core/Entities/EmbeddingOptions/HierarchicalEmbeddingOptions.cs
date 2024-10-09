namespace Pixion.LearnRag.Core.Entities.EmbeddingOptions;

public record HierarchicalEmbeddingOptions(
    int ChunkSize,
    int ChunkOverlap,
    int ChildChunkSize,
    int ChildChunkOverlap);