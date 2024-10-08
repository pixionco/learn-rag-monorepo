using Pixion.LearnRag.Core.Models;
using Pixion.LearnRag.Core.Services;

namespace Pixion.LearnRag.UseCases.MockServices;

public class QuestionGenerationServiceMock : IQuestionGenerationService
{
    public async Task<QuestionGenerationResult> GenerateQuestionsAsync(string text, int numberOfQuestion)
    {
        var questions = text
            .Split(" ")
            .Take(numberOfQuestion)
            .ToList();

        // If the number of questions is less than the required number, add a default question
        while (questions.Count < numberOfQuestion)
            questions.Add("What is the meaning of life?");

        var successResult = new QuestionGenerationSuccessResult(questions, text.Length, questions.Sum(x => x.Length));

        return await Task.FromResult<QuestionGenerationResult>(successResult);
    }
}