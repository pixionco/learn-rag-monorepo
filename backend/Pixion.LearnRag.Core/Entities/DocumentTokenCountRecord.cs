namespace Pixion.LearnRag.Core.Entities;

public record DocumentTokenCountRecord(Guid Id, Guid DocumentId, string ModelName, int TokenCount);