using System.Net;

namespace Pixion.LearnRag.Core.Models;

public record EmbeddingGenerationErrorResult(Exception Exception, HttpStatusCode? StatusCode)
    : EmbeddingGenerationResult;