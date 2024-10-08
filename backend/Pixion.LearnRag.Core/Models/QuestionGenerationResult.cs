namespace Pixion.LearnRag.Core.Models;

public abstract record QuestionGenerationResult(int InputTokenCount)
{
    public bool IsSuccess => this is QuestionGenerationSuccessResult;
}