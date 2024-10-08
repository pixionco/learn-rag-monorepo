using Ardalis.Result;
using Ardalis.SharedKernel;
using Pixion.LearnRag.Core.Enums;
using Pixion.LearnRag.Core.Services.RetrievalStrategy;

namespace Pixion.LearnRag.UseCases.Features.Embed;

public class EmbedHandler(
    IBasicStrategyService basicStrategyService,
    ISentenceWindowStrategyService sentenceWindowStrategyService,
    IAutoMergingStrategyService autoMergingStrategyService,
    IHierarchicalStrategyService hierarchicalStrategyService,
    IHypotheticalQuestionStrategyService hypotheticalQuestionStrategyService
) : ICommandHandler<EmbedCommand, Result>
{
    public async Task<Result> Handle(EmbedCommand request, CancellationToken cancellationToken)
    {
        /*foreach (var embedCommandOptions in request.EmbedOptionsEnumerable)
        {
            var (strategy, chunkSize, chunkOverlap, childChunkSize, childChunkOverlap, numberOfQuestions) =
                embedCommandOptions;
            var operations = strategy switch
            {
                _ when strategy.Equals(Strategy.Basic) =>
                    new[] { basicStrategyService.GetOperations(chunkSize, chunkOverlap!) },
                _ when strategy.Equals(Strategy.SentenceWindow) => new[]
                {
                    sentenceWindowStrategyService.GetOperations(chunkSize)
                },
                _ when strategy.Equals(Strategy.AutoMerging) => autoMergingStrategyService.GetOperations(
                    chunkSize,
                    chunkOverlap!,
                    childChunkSize!,
                    childChunkOverlap!
                ),
                _ when strategy.Equals(Strategy.Hierarchical) => hierarchicalStrategyService.GetOperations(
                    chunkSize,
                    chunkOverlap!,
                    childChunkSize!,
                    childChunkOverlap!
                ),
                _ when strategy.Equals(Strategy.HypotheticalQuestion) => new[]
                {
                    hypotheticalQuestionStrategyService.GetOperations(chunkSize, chunkOverlap!, numberOfQuestions)
                },
                _ => throw new ArgumentOutOfRangeException(strategy.Name, strategy, "Unknown strategy!")
            };
            allOperations.Add(operations);
        }

        var jaggedOperations = allOperations.SelectMany(outer => outer.Select(inner => inner));
        var operationTree = OperationTree.Create(request.DocumentId, jaggedOperations);
        operationTree.Print();

        await operationTreeRepository.InsertAsync(operationTree, cancellationToken);
        await operationTreeNodeRepository.BatchInsertAsync(operationTree.Flatten(), cancellationToken);

        var res = await operationTreeRepository.FindByIdAsync(operationTree.Id, cancellationToken);
        return Result<OperationTree?>.Success(res);*/
        return Result.Success();
    }
}