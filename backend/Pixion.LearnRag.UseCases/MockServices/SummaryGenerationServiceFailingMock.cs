using Pixion.LearnRag.Core.Models;
using Pixion.LearnRag.Core.Services;

namespace Pixion.LearnRag.UseCases.MockServices;

public class SummaryGenerationServiceFailingMock : ISummaryGenerationService
{
    public async Task<SummaryGenerationResult> GenerateSummaryAsync(string text)
    {
        var isSuccess = Convert.ToBoolean(new Random().Next(0, 2));
        var successResult = new SummaryGenerationSuccessResult(text, text.Length, text.Length / 2);
        var errorResult = new SummaryGenerationErrorResult(
            new Exception("Summary generation failed!"),
            text.Length,
            null,
            null
        );

        return await Task.FromResult<SummaryGenerationResult>(isSuccess ? successResult : errorResult);
    }
}