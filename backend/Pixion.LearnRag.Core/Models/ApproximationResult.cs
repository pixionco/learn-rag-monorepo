namespace Pixion.LearnRag.Core.Models;

public record ApproximationResult(
    string OperationPath,
    double InputTokenCountApproximation,
    double OutputTokenCountApproximation,
    double FailedInputTokenCountApproximation
);