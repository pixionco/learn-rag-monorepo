using Microsoft.Extensions.Options;
using Npgsql;
using Pixion.LearnRag.Infrastructure.Configs;
using Pixion.LearnRag.Infrastructure.Interfaces;

namespace Pixion.LearnRag.Infrastructure.Seeders;

public class PostgresEmbeddingRecordSeeder : ISeeder
{
    private readonly string _embeddingRecordTableName;
    private readonly NpgsqlDataSource _npgsql;
    private readonly string _schema;
    private readonly int _vectorSize;

    public PostgresEmbeddingRecordSeeder(IOptions<PostgresVectorRepositoryConfig> config)
    {
        _schema = config.Value.Schema;
        _vectorSize = config.Value.VectorSize;
        _embeddingRecordTableName = config.Value.EmbeddingRecordTableName;

        var dataSourceBuilder = new NpgsqlDataSourceBuilder(config.Value.DatabaseConnection);
        dataSourceBuilder.UseVector();
        _npgsql = dataSourceBuilder.Build();
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        var connection = await _npgsql.OpenConnectionAsync(cancellationToken);

        await using (connection)
        {
            await using var tableCmd = connection.CreateCommand();
            tableCmd.CommandText = $"""
                                    CREATE TABLE IF NOT EXISTS {_schema}.{_embeddingRecordTableName} (
                                        id UUID NOT NULL PRIMARY KEY,
                                        text TEXT NOT NULL,
                                        embedding vector({_vectorSize}) NOT NULL,
                                        operations VARCHAR(255) NOT NULL,
                                        document_id UUID NOT NULL,
                                        operation_tree_id UUID NOT NULL,
                                        index INT NOT NULL,
                                        parent_index INT,
                                        parent_chunk TEXT
                                    )
                                    """;
            await tableCmd.ExecuteNonQueryAsync(cancellationToken);

            await using var indexCmd = connection.CreateCommand();
            indexCmd.CommandText = $"""
                                    CREATE INDEX IF NOT EXISTS {_embeddingRecordTableName}_index
                                    ON {_schema}.{_embeddingRecordTableName}
                                    USING hnsw (embedding vector_cosine_ops) WITH (m = 16, ef_construction = 64);

                                    CREATE INDEX IF NOT EXISTS {_embeddingRecordTableName}_ops_doc_id_par_idx
                                    ON {_schema}.{_embeddingRecordTableName} (operations, document_id, parent_index);

                                    CREATE INDEX IF NOT EXISTS {_embeddingRecordTableName}_ops_doc_id_idx
                                    ON {_schema}.{_embeddingRecordTableName} (operations, document_id, index);
                                    """;
            await indexCmd.ExecuteNonQueryAsync(cancellationToken);
        }
    }
}