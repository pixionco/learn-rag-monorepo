using Microsoft.Extensions.Options;
using Pgvector;
using Pixion.LearnRag.Core.Enums;
using Pixion.LearnRag.Core.Models;
using Pixion.LearnRag.Core.Repositories;
using Pixion.LearnRag.Infrastructure.Configs;

namespace Pixion.LearnRag.Infrastructure.Repositories;

public class AutoMergingStrategyRepository(IOptions<PostgresVectorRepositoryConfig> config)
    : StrategyRepository(config, Strategy.AutoMerging.Name), IAutoMergingStrategyRepository
{
    public async Task<IEnumerable<SearchResult>> GetParentChunksAsync(Guid documentId, IEnumerable<int> parentIndexes,
        CancellationToken cancellationToken)
    {
        var searchResults = new List<SearchResult>();
        var connection = await Npgsql.OpenConnectionAsync(cancellationToken);

        await using (connection)
        {
            await using var cmd = connection.CreateCommand();
            cmd.CommandText = $"""
                               SELECT *
                               FROM {Table}
                               WHERE
                                 document_id = @t1 AND
                                 index = ANY(@t2)
                               """;
            cmd.Parameters.AddWithValue("@t1", documentId);
            cmd.Parameters.AddWithValue("@t2", parentIndexes.ToArray());

            await using var dataReader = await cmd.ExecuteReaderAsync(cancellationToken);
            while (await dataReader.ReadAsync(cancellationToken)) searchResults.Add(ReadSearchResult(dataReader));
        }

        return searchResults;
    }

    public async Task<IEnumerable<SearchResult>> SearchLeafChunksAsync(ReadOnlyMemory<float> queryEmbedding,
        int limit,
        CancellationToken cancellationToken = default)
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
                                    metadata ? 'ParentIndex'
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