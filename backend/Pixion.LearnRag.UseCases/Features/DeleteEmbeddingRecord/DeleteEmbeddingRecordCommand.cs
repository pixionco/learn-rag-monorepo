using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Pixion.LearnRag.UseCases.Features.DeleteEmbeddingRecord;

public record DeleteEmbeddingRecordCommand(Guid OperationTreeId) : ICommand<Result<int>>;