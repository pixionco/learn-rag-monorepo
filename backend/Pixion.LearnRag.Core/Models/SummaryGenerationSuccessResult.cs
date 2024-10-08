namespace Pixion.LearnRag.Core.Models;

public record SummaryGenerationSuccessResult(
    string Summary,
    int InputTokenCount,
    int OutputTokenCount
) : SummaryGenerationResult(InputTokenCount);