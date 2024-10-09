using Ardalis.Result;
using Ardalis.SharedKernel;
using Pixion.LearnRag.Core.Services.RetrievalStrategy;

namespace Pixion.LearnRag.UseCases.Features.Embed;

public class EmbedBasicHandler(
    IBasicStrategyService basicStrategyService
) : ICommandHandler<EmbedBasicCommand, Result>
{
    public async Task<Result> Handle(EmbedBasicCommand request, CancellationToken cancellationToken)
    {
        var (documentId, embeddingOptions) = request;

        await basicStrategyService.EmbedAsync(documentId, embeddingOptions);

        return Result.Success();
    }
}