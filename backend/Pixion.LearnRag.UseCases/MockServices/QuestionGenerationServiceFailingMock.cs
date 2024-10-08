using Pixion.LearnRag.Core.Models;
using Pixion.LearnRag.Core.Services;

namespace Pixion.LearnRag.UseCases.MockServices;

public class QuestionGenerationServiceFailingMock : IQuestionGenerationService
{
    public async Task<QuestionGenerationResult> GenerateQuestionsAsync(string text, int numberOfQuestion)
    {
        var isSuccess = Convert.ToBoolean(new Random().Next(0, 2));
        if (!isSuccess)
        {
            var errorResult = new QuestionGenerationErrorResult(
                new Exception("Question generation failed!"),
                text.Length,
                null,
                null
            );

            return await Task.FromResult<QuestionGenerationResult>(errorResult);
        }

        var takeCount = new Random().Next(0, numberOfQuestion + 1);

        var questions = text
            .Split(" ")
            .Take(numberOfQuestion)
            .ToList();

        // If the number of questions is less than the required number, add a default question
        while (questions.Count < numberOfQuestion)
            questions.Add("What is the meaning of life?");

        questions = questions.Take(takeCount).ToList();

        var successResult = new QuestionGenerationSuccessResult(questions, text.Length, questions.Sum(x => x.Length));

        return await Task.FromResult<QuestionGenerationResult>(successResult);
    }
}