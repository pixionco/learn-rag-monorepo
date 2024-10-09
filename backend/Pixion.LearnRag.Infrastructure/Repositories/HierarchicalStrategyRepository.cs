using Microsoft.Extensions.Options;
using Pgvector;
using Pixion.LearnRag.Core.Enums;
using Pixion.LearnRag.Core.Models;
using Pixion.LearnRag.Core.Repositories;
using Pixion.LearnRag.Infrastructure.Configs;

namespace Pixion.LearnRag.Infrastructure.Repositories;

public class HierarchicalStrategyRepository(IOptions<PostgresVectorRepositoryConfig> config)
    : StrategyRepository(config, Strategy.Hierarchical.Name), IHierarchicalStrategyRepository
{
    public async Task<IEnumerable<SearchResult>> SearchByParentAsync(
        ReadOnlyMemory<float> queryEmbedding,
        Guid documentId,
        int parentIndex,
        int limit,
        CancellationToken cancellationToken
    )
    {
        var searchResults = new List<SearchResult>();
        var connection = await Npgsql.OpenConnectionAsync(cancellationToken);

        await using (connection)
        {
            await using var cmd = connection.CreateCommand();
            cmd.CommandText = $$"""
                                SELECT *
                                FROM (
                                  SELECT *, 1 - (embedding <=> @t1) AS cosine_similarity
                                  FROM {{Table}}
                                  WHERE
                                    document_id = @t2 AND
                                    metadata @> '{DocumentId = @t3}'
                                ) AS relevance_table
                                ORDER BY
                                    cosine_similarity DESC
                                LIMIT @t4
                                """;
            cmd.Parameters.AddWithValue("@t1", new Vector(queryEmbedding));
            cmd.Parameters.AddWithValue("@t2", documentId);
            cmd.Parameters.AddWithValue("@t3", parentIndex);
            cmd.Parameters.AddWithValue("@t4", limit);

            await using var dataReader = await cmd.ExecuteReaderAsync(cancellationToken);

            while (await dataReader.ReadAsync(cancellationToken)) searchResults.Add(ReadSearchResult(dataReader));
        }

        return searchResults;
    }

    public async Task<IEnumerable<SearchResult>> SearchFirstLevelAsync(ReadOnlyMemory<float> queryEmbedding,
        int limit,
        CancellationToken cancellationToken)
    {
        var searchResults = new List<SearchResult>();
        var connection = await Npgsql.OpenConnectionAsync(cancellationToken);

        await using (connection)
        {
            await using var cmd = connection.CreateCommand();
            cmd.CommandText = $$"""
                                SELECT *
                                FROM (
                                  SELECT *, 1 - (embedding <=> @t1) AS cosine_similarity
                                  FROM {{Table}}
                                  WHERE
                                    NOT metadata ? 'ParentIndex'
                                ) AS relevance_table
                                ORDER BY
                                    cosine_similarity DESC
                                LIMIT @t2
                                """;
            cmd.Parameters.AddWithValue("@t1", new Vector(queryEmbedding));
            cmd.Parameters.AddWithValue("@t2", limit);

            await using var dataReader = await cmd.ExecuteReaderAsync(cancellationToken);

            while (await dataReader.ReadAsync(cancellationToken)) searchResults.Add(ReadSearchResult(dataReader));
        }

        return searchResults;
    }
}