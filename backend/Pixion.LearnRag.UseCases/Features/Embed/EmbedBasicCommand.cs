using Ardalis.Result;
using Ardalis.SharedKernel;
using Pixion.LearnRag.Core.Entities.EmbeddingOptions;

namespace Pixion.LearnRag.UseCases.Features.Embed;

public record EmbedBasicCommand(
    Guid DocumentId,
    BasicEmbeddingOptions EmbeddingOptions)
    : ICommand<Result>;