namespace Pixion.LearnRag.Infrastructure.Configs;

public class PostgresVectorRepositoryConfig
{
    public string DatabaseConnection { get; init; } = string.Empty;
    public int VectorSize { get; init; }
    public string Schema { get; init; } = string.Empty;
}