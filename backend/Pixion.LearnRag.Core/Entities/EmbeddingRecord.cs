namespace Pixion.LearnRag.Core.Entities;

public record EmbeddingRecord(
    Guid Id,
    string Text,
    ReadOnlyMemory<float> Embedding,
    string Operations,
    Metadata Metadata
);