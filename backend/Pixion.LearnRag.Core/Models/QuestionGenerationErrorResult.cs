using System.Net;

namespace Pixion.LearnRag.Core.Models;

public record QuestionGenerationErrorResult(
    Exception Exception,
    int InputTokenCount,
    string? ResponseString,
    HttpStatusCode? StatusCode
) : QuestionGenerationResult(InputTokenCount);