using Pixion.LearnRag.Core.Models;

namespace Pixion.LearnRag.Core.Services;

public interface ISummaryGenerationService
{
    public Task<SummaryGenerationResult> GenerateSummaryAsync(string text);
}