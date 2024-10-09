namespace Pixion.LearnRag.Core.Entities;

public record HierarchicalMetadata(Guid DocumentId, int Index, int? ParentIndex) : BasicMetadata(DocumentId, Index);