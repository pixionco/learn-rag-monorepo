using Pixion.LearnRag.Core.Enums;

namespace Pixion.LearnRag.UseCases.Features;

public record EmbedOptions(
    Strategy Strategy,
    ChunkSize ChunkSize,
    ChunkOverlap? ChunkOverlap,
    ChunkSize? ChildChunkSize,
    ChunkOverlap? ChildChunkOverlap,
    int? NumberOfQuestions
);