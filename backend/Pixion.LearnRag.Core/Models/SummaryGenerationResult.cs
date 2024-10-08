namespace Pixion.LearnRag.Core.Models;

public abstract record SummaryGenerationResult(int InputTokenCount)
{
    public bool IsSuccess => this is SummaryGenerationSuccessResult;
}