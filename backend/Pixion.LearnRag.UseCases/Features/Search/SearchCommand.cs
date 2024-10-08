using Ardalis.Result;
using Ardalis.SharedKernel;
using Pixion.LearnRag.Core.Enums;
using Pixion.LearnRag.Core.Models;

namespace Pixion.LearnRag.UseCases.Features.Search;

public record SearchCommand(
    Strategy Strategy,
    string Query,
    ChunkSize ChunkSize,
    ChunkOverlap? ChunkOverlap,
    ChunkSize? ChildChunkSize,
    ChunkOverlap? ChildChunkOverlap,
    int? Range,
    double? ChildParentPrevalenceFactor,
    int Limit,
    int? ChildLimit
) : ICommand<Result<SearchResponse>>;