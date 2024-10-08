using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Pixion.LearnRag.UseCases.Features.Embed;

public record EmbedCommand(Guid DocumentId, IEnumerable<EmbedOptions> EmbedOptionsEnumerable)
    : ICommand<Result>;