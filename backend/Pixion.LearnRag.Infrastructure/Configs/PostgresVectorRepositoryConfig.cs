namespace Pixion.LearnRag.Infrastructure.Configs;

public class PostgresVectorRepositoryConfig
{
    public string DatabaseConnection { get; init; } = string.Empty;
    public int VectorSize { get; init; }
    public string Schema { get; init; } = string.Empty;
    public string EmbeddingRecordTableName { get; init; } = string.Empty;
    public string AiModelFailedMessageTableName { get; init; } = string.Empty;
    public string OperationPathTableName { get; init; } = string.Empty;
}