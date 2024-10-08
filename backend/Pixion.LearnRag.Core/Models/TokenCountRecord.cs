namespace Pixion.LearnRag.Core.Models;

public record TokenCountRecord(
    int InputTokenCount,
    int OutputTokenCount,
    int FailedInputTokenCount
);