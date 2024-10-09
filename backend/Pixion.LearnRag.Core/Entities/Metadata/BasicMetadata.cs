using Pixion.LearnRag.Core.Entities.EmbeddingOptions;

namespace Pixion.LearnRag.Core.Entities.Metadata;

public record BasicMetadata(Guid DocumentId, int Index, int ChunkSize, int ChunkOverlap)
    : BasicEmbeddingOptions(ChunkOverlap, ChunkOverlap), IMetadataBase;