using Pixion.LearnRag.Core.Entities.EmbeddingOptions;

namespace Pixion.LearnRag.Core.Entities.Metadata;

public record AutoMergingMetadata(
    Guid DocumentId,
    int Index,
    int? ParentIndex,
    int ChunkSize,
    int ChunkOverlap,
    int ChildChunkSize,
    int ChildChunkOverlap
) : AutoMergingEmbeddingOptions(
    ChunkSize,
    ChunkOverlap,
    ChildChunkSize,
    ChildChunkOverlap
), IMetadataBase;