namespace Pixion.LearnRag.Core.Models;

public record QuestionGenerationSuccessResult(
    IEnumerable<string> Questions,
    int InputTokenCount,
    int OutputTokenCount
) : QuestionGenerationResult(InputTokenCount);