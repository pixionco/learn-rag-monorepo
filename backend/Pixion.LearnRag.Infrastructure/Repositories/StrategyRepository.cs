using Microsoft.Extensions.Options;
using Npgsql;
using Pgvector;
using Pixion.LearnRag.Core.Entities;
using Pixion.LearnRag.Core.Models;
using Pixion.LearnRag.Core.Repositories;
using Pixion.LearnRag.Infrastructure.Configs;

namespace Pixion.LearnRag.Infrastructure.Repositories;

public abstract class StrategyRepository : IStrategyRepository
{
    protected readonly NpgsqlDataSource Npgsql;
    protected readonly string Table;

    protected StrategyRepository(IOptions<PostgresVectorRepositoryConfig> config, string table)
    {
        var dataSourceBuilder = new NpgsqlDataSourceBuilder(config.Value.DatabaseConnection);
        dataSourceBuilder.UseVector();
        Npgsql = dataSourceBuilder.Build();
        Table = table;
    }

    public async Task InsertAsync(EmbeddingRecord embeddingRecord, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task BatchInsertAsync(IEnumerable<EmbeddingRecord> embeddingRecords,
        CancellationToken cancellationToken = default)
    {
        var connection = await Npgsql.OpenConnectionAsync(cancellationToken);

        await using (connection)
        {
            var embeddingRecordsList = embeddingRecords.ToList();

            await using var cmd = connection.CreateCommand();
            cmd.CommandText = $"""
                               INSERT INTO {Table} (
                                 id, text, embedding, metadata
                               )
                               SELECT bulk.*
                               FROM (
                                 SELECT * FROM unnest(@t1, @t2, @t3, @t4, @t5) AS
                                   t(id, text, embedding, document_id, metadata)
                               ) AS bulk
                               """;

            cmd.Parameters.AddWithValue(
                "@t1",
                embeddingRecordsList
                    .Select(x => x.Id)
                    .ToArray()
            );
            cmd.Parameters.AddWithValue(
                "@t2",
                embeddingRecordsList
                    .Select(x => x.Text)
                    .ToArray()
            );
            cmd.Parameters.AddWithValue(
                "@t3",
                embeddingRecordsList
                    .Select(x => new Vector(x.Embedding))
                    .ToArray()
            );
            cmd.Parameters.AddWithValue(
                "@t4",
                embeddingRecordsList
                    .Select(x => x.MetadataString)
                    .ToArray()
            );

            await cmd.ExecuteNonQueryAsync(cancellationToken);
        }
    }

    public async Task<IEnumerable<SearchResult>> SearchAsync(ReadOnlyMemory<float> queryEmbedding, int limit,
        CancellationToken cancellationToken = default)
    {
        var searchResults = new List<SearchResult>();
        var connection = await Npgsql.OpenConnectionAsync(cancellationToken);

        await using (connection)
        {
            await using var cmd = connection.CreateCommand();
            cmd.CommandText = $"""
                               SELECT *
                               FROM (
                                 SELECT *, 1 - (embedding <=> @t1) AS cosine_similarity
                                 FROM {Table}
                               ) AS relevance_table
                               ORDER BY
                                   cosine_similarity DESC
                               LIMIT @t2
                               """;
            cmd.Parameters.AddWithValue("@t1", new Vector(queryEmbedding));
            cmd.Parameters.AddWithValue("@t2", limit);

            await using var dataReader = await cmd.ExecuteReaderAsync(cancellationToken);

            while (await dataReader.ReadAsync(cancellationToken)) searchResults.Add(ReadSearchResult(dataReader));

            return searchResults;
        }
    }

    protected SearchResult ReadSearchResult(NpgsqlDataReader dataReader)
    {
        var text = dataReader.GetString(dataReader.GetOrdinal("key"));
        var metadataString = dataReader.GetString(dataReader.GetOrdinal("metadata"));

        double? relevance = null;
        var relevanceOrdinal = dataReader.GetOrdinal("cosine_similarity");
        if (!dataReader.IsDBNull(relevanceOrdinal)) relevance = dataReader.GetDouble(relevanceOrdinal);

        return new SearchResult(text, relevance, metadataString);
    }
}