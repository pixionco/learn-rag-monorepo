using Pixion.LearnRag.Core.Models;
using Pixion.LearnRag.Core.Services;

namespace Pixion.LearnRag.UseCases.MockServices;

public class SummaryGenerationServiceMock : ISummaryGenerationService
{
    public async Task<SummaryGenerationResult> GenerateSummaryAsync(string text)
    {
        var successResult = new SummaryGenerationSuccessResult(text, text.Length, text.Length / 2);

        return await Task.FromResult<SummaryGenerationResult>(successResult);
    }
}