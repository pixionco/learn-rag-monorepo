namespace Pixion.LearnRag.Core.Entities.Metadata;

public interface IMetadataBase
{
    Guid DocumentId { get; init; }
    int Index { get; init; }
}