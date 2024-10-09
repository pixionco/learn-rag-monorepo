using Pixion.LearnRag.Core.Entities.EmbeddingOptions;

namespace Pixion.LearnRag.Core.Entities.Metadata;

public record HypotheticalQuestionMetadata(
    Guid DocumentId,
    int Index,
    int chunkSize,
    int chunkOverlap,
    int numOfQuestions
) : HypotheticalQuestionEmbeddingOptions(
    chunkSize,
    chunkOverlap,
    numOfQuestions
), IMetadataBase;