using Pixion.LearnRag.Core.Entities.EmbeddingOptions;

namespace Pixion.LearnRag.Core.Entities.Metadata;

public record SentenceWindowMetadata(Guid DocumentId, int Index, int ChunkSize)
    : SentenceWindowEmbeddingOptions(ChunkSize), IMetadataBase;