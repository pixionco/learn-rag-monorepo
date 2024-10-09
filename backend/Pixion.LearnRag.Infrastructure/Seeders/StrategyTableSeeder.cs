using System.Collections.Immutable;
using Microsoft.Extensions.Options;
using Npgsql;
using Pixion.LearnRag.Core.Enums;
using Pixion.LearnRag.Infrastructure.Configs;
using Pixion.LearnRag.Infrastructure.Interfaces;

namespace Pixion.LearnRag.Infrastructure.Seeders;

public class StrategyTableSeeder : ISeeder
{
    private readonly NpgsqlDataSource _npgsql;
    private readonly string _schema;
    private readonly int _vectorSize;

    public StrategyTableSeeder(IOptions<PostgresVectorRepositoryConfig> config)
    {
        _schema = config.Value.Schema;
        _vectorSize = config.Value.VectorSize;

        var dataSourceBuilder = new NpgsqlDataSourceBuilder(config.Value.DatabaseConnection);
        dataSourceBuilder.UseVector();
        _npgsql = dataSourceBuilder.Build();
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        var connection = await _npgsql.OpenConnectionAsync(cancellationToken);
        var strategyNames = Strategy.List.Select(strategy => strategy.Name).ToImmutableArray();

        // creating index on vector column while the table has low entry count
        // only increases memory and reduces accuracy without performance benefits
        await using (connection)
        {
            await using var tableCmd = connection.CreateCommand();
            foreach (var strategyName in strategyNames)
            {
                tableCmd.CommandText = $"""
                                        CREATE TABLE IF NOT EXISTS {_schema}.{strategyName} (
                                            id UUID NOT NULL PRIMARY KEY,
                                            text TEXT NOT NULL,
                                            embedding vector({_vectorSize}) NOT NULL,
                                            metadata JSONB
                                        )
                                        """;
                await tableCmd.ExecuteNonQueryAsync(cancellationToken);
            }
        }
    }
}