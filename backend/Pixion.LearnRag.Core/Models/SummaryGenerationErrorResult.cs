using System.Net;

namespace Pixion.LearnRag.Core.Models;

public record SummaryGenerationErrorResult(
    Exception Exception,
    int InputTokenCount,
    string? ResponseString,
    HttpStatusCode? StatusCode
) : SummaryGenerationResult(InputTokenCount);