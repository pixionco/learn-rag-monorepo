namespace Pixion.LearnRag.Core.Models;

public record StepCountRecord(
    int AssignedStepCount,
    int CompletedStepCount,
    int SkippedStepCount,
    int ErroredStepCount
);