namespace Pixion.LearnRag.Core.Entities;

public record Metadata
{
    public Guid DocumentId { init; get; }
    public int Index { init; get; }
    public int? ParentIndex { init; get; }
    public string? ParentChunk { init; get; }
}