using Ardalis.Result;
using Ardalis.SharedKernel;
using Pixion.LearnRag.Core.Repositories;

namespace Pixion.LearnRag.UseCases.Features.DeleteEmbeddingRecord;

public class DeleteEmbeddingRecordHandler(IEmbeddingRecordRepository embeddingRecordRepository)
    : ICommandHandler<DeleteEmbeddingRecordCommand, Result<int>>
{
    public async Task<Result<int>> Handle(DeleteEmbeddingRecordCommand request, CancellationToken cancellationToken)
    {
        var deletedItems =
            await embeddingRecordRepository.DeleteByOperationTreeIdAsync(request.OperationTreeId, cancellationToken);

        return Result<int>.Success(deletedItems);
    }
}