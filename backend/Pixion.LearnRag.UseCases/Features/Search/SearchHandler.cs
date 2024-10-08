using System.Diagnostics;
using Ardalis.Result;
using Ardalis.SharedKernel;
using Pixion.LearnRag.Core.Clients;
using Pixion.LearnRag.Core.Enums;
using Pixion.LearnRag.Core.Models;
using Pixion.LearnRag.Core.Services.RetrievalStrategy;

namespace Pixion.LearnRag.UseCases.Features.Search;

public class SearchHandler(
    IBasicStrategyService basicStrategyService,
    ISentenceWindowStrategyService sentenceWindowStrategyService,
    IAutoMergingStrategyService autoMergingStrategyService,
    IHierarchicalStrategyService hierarchicalStrategyService,
    IHypotheticalQuestionStrategyService hypotheticalQuestionStrategyService,
    ITokenizingClient tokenizingClient
) : ICommandHandler<SearchCommand, Result<SearchResponse>>
{
    public async Task<Result<SearchResponse>> Handle(
        SearchCommand request,
        CancellationToken cancellationToken
    )
    {
        var stopwatch = Stopwatch.StartNew();
        var inputTokenCount = tokenizingClient.GetEmbeddingModelTokenCount(request.Query);
        var searchResults = request.Strategy switch
        {
            var s when s.Equals(Strategy.Basic) =>
                await basicStrategyService.SearchAsync(
                    request.ChunkSize,
                    request.ChunkOverlap!,
                    request.Query,
                    request.Limit
                ),
            var s when s.Equals(Strategy.SentenceWindow) =>
                await sentenceWindowStrategyService.SearchAsync(
                    request.ChunkSize,
                    request.Query,
                    request.Range!.Value,
                    request.Limit
                ),
            var s when s.Equals(Strategy.AutoMerging) =>
                await autoMergingStrategyService.SearchAsync(
                    request.ChunkSize,
                    request.ChunkOverlap!,
                    request.ChildChunkSize!,
                    request.ChildChunkOverlap!,
                    request.ChildParentPrevalenceFactor!.Value,
                    request.Query,
                    request.Limit
                ),
            var s when s.Equals(Strategy.Hierarchical) =>
                await hierarchicalStrategyService.SearchAsync(
                    request.ChunkSize,
                    request.ChunkOverlap!,
                    request.ChildChunkSize!,
                    request.ChildChunkOverlap!,
                    request.Query,
                    request.Limit,
                    request.ChildLimit!.Value
                ),
            var s when s.Equals(Strategy.HypotheticalQuestion) =>
                await hypotheticalQuestionStrategyService.SearchAsync(
                    request.ChunkSize,
                    request.ChunkOverlap!,
                    request.Query,
                    request.Limit
                ),
            _ => throw new ArgumentOutOfRangeException()
        };
        stopwatch.Stop();

        var searchResponse = new SearchResponse(searchResults, inputTokenCount, stopwatch.ElapsedMilliseconds);

        return Result.Success(searchResponse);
    }
}