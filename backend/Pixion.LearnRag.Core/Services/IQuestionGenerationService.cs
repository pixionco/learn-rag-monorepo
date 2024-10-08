using Pixion.LearnRag.Core.Models;

namespace Pixion.LearnRag.Core.Services;

public interface IQuestionGenerationService
{
    public Task<QuestionGenerationResult> GenerateQuestionsAsync(
        string text,
        int numberOfQuestion
    );
}