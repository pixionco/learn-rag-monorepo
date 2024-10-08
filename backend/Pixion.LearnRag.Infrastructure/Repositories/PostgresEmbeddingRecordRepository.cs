using Microsoft.Extensions.Options;
using Npgsql;
using NpgsqlTypes;
using Pgvector;
using Pixion.LearnRag.Core.Entities;
using Pixion.LearnRag.Core.Models;
using Pixion.LearnRag.Core.Repositories;
using Pixion.LearnRag.Infrastructure.Configs;

namespace Pixion.LearnRag.Infrastructure.Repositories;

#pragma warning disable SKEXP0032

public class PostgresEmbeddingRecordRepository : IEmbeddingRecordRepository
{
    private readonly string _embeddingRecordTableName;
    private readonly NpgsqlDataSource _npgsql;

    public PostgresEmbeddingRecordRepository(IOptions<PostgresVectorRepositoryConfig> config)
    {
        var dataSourceBuilder = new NpgsqlDataSourceBuilder(config.Value.DatabaseConnection);
        dataSourceBuilder.UseVector();
        _npgsql = dataSourceBuilder.Build();

        _embeddingRecordTableName = config.Value.EmbeddingRecordTableName;
    }

    public Task InsertOneAsync(EmbeddingRecord embeddingRecord, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task InsertManyAsync(IEnumerable<EmbeddingRecord> embeddingRecords,
        CancellationToken cancellationToken)
    {
        var connection = await _npgsql.OpenConnectionAsync(cancellationToken);

        await using (connection)
        {
            var embeddingRecordsList = embeddingRecords.ToList();
            var metadataList = embeddingRecordsList
                .Select(x => x.Metadata)
                .ToList();

            await using var cmd = connection.CreateCommand();
            cmd.CommandText = $"""
                               INSERT INTO {_embeddingRecordTableName} (
                                 id, text, embedding, operations, document_id, operation_tree_id, index, parent_index, parent_chunk
                               )
                               SELECT bulk.*
                               FROM (
                                 SELECT * FROM unnest(@t1, @t2, @t3, @t4, @t5, @t6, @t7, @t8, @t9) AS
                                   t(id, text, embedding, operations, document_id, operation_tree_id, index, parent_index, parent_chunk)
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
                    .Select(x => x.Operations)
                    .ToArray()
            );
            cmd.Parameters.AddWithValue(
                "@t5",
                metadataList
                    .Select(x => x.DocumentId)
                    .ToArray()
            );
            cmd.Parameters.AddWithValue(
                "@t6",
                metadataList
                    .Select(x => x.OperationTreeId)
                    .ToArray()
            );
            cmd.Parameters.AddWithValue(
                "@t7",
                metadataList
                    .Select(x => x.Index)
                    .ToArray()
            );
            cmd.Parameters.AddWithValue(
                "@t8",
                metadataList
                    .Select(x => x.ParentIndex)
                    .ToArray()
            );
            cmd.Parameters.AddWithValue(
                "@t9",
                metadataList
                    .Select(x => x.ParentChunk)
                    .ToArray()
            );

            await cmd.ExecuteNonQueryAsync(cancellationToken);
        }
    }

    public async Task<IEnumerable<SearchResult>> SearchAsync(
        ReadOnlyMemory<float> queryEmbedding,
        int operationPathId,
        int limit,
        CancellationToken cancellationToken
    )
    {
        var searchResults = new List<SearchResult>();
        var connection = await _npgsql.OpenConnectionAsync(cancellationToken);

        await using (connection)
        {
            await using var cmd = connection.CreateCommand();
            cmd.CommandText = $"""
                               SELECT *
                               FROM (
                                 SELECT *, 1 - (embedding <=> @t1) AS cosine_similarity
                                 FROM {_embeddingRecordTableName}_{operationPathId}
                               ) AS relevance_table
                               ORDER BY
                                   cosine_similarity DESC
                               LIMIT @t2
                               """;
            cmd.Parameters.AddWithValue("@t1", new Vector(queryEmbedding));
            cmd.Parameters.AddWithValue("@t2", limit);

            await using var dataReader = await cmd.ExecuteReaderAsync(cancellationToken);

            while (await dataReader.ReadAsync(cancellationToken))
            {
                var text = dataReader.GetString(dataReader.GetOrdinal("text"));
                var relevance = dataReader.GetDouble(dataReader.GetOrdinal("cosine_similarity"));

                var parentIndexOrdinal = dataReader.GetOrdinal("parent_index");
                var parentChunkOrdinal = dataReader.GetOrdinal("parent_chunk");
                var isParentIndexNull = await dataReader.IsDBNullAsync(parentIndexOrdinal, cancellationToken);
                var isParentChunkNull = await dataReader.IsDBNullAsync(parentChunkOrdinal, cancellationToken);

                var metadata = new Metadata
                {
                    DocumentId = dataReader.GetGuid(dataReader.GetOrdinal("document_id")),
                    OperationTreeId = dataReader.GetGuid(dataReader.GetOrdinal("operation_tree_id")),
                    Index = dataReader.GetInt32(dataReader.GetOrdinal("index")),
                    ParentIndex = isParentIndexNull ? null : dataReader.GetInt32(parentIndexOrdinal),
                    ParentChunk = isParentChunkNull ? null : dataReader.GetString(parentChunkOrdinal)
                };
                var searchResult = new SearchResult(text, relevance, metadata);
                searchResults.Add(searchResult);
            }

            return searchResults;
        }
    }

    public async Task<IEnumerable<SearchResult>> SearchBySummaryAsync(
        ReadOnlyMemory<float> queryEmbedding,
        int operationPathId,
        Guid documentId,
        int parentIndex,
        int limit,
        CancellationToken cancellationToken
    )
    {
        var searchResults = new List<SearchResult>();
        var connection = await _npgsql.OpenConnectionAsync(cancellationToken);

        await using (connection)
        {
            await using var cmd = connection.CreateCommand();
            cmd.CommandText = $"""
                               SELECT *
                               FROM (
                                 SELECT *, 1 - (embedding <=> @t1) AS cosine_similarity
                                 FROM {_embeddingRecordTableName}_{operationPathId}
                                 WHERE
                                   document_id = @t2 AND
                                   parent_index = @t3
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

            while (await dataReader.ReadAsync(cancellationToken))
            {
                var text = dataReader.GetString(dataReader.GetOrdinal("text"));
                var relevance = dataReader.GetDouble(dataReader.GetOrdinal("cosine_similarity"));
                var metadata = new Metadata
                {
                    DocumentId = dataReader.GetGuid(dataReader.GetOrdinal("document_id")),
                    OperationTreeId = dataReader.GetGuid(dataReader.GetOrdinal("operation_tree_id")),
                    Index = dataReader.GetInt32(dataReader.GetOrdinal("index")),
                    ParentIndex = dataReader.GetInt32(dataReader.GetOrdinal("parent_index")),
                    ParentChunk = dataReader.GetString(dataReader.GetOrdinal("parent_chunk"))
                };
                var searchResult = new SearchResult(text, relevance, metadata);
                searchResults.Add(searchResult);
            }
        }

        return searchResults;
    }

    public async Task<IEnumerable<SearchResult>> GetParentChunksAsync(
        IEnumerable<int> parentIndexes,
        Guid documentId,
        int operationPathId,
        CancellationToken cancellationToken
    )
    {
        var searchResults = new List<SearchResult>();
        var connection = await _npgsql.OpenConnectionAsync(cancellationToken);

        await using (connection)
        {
            await using var cmd = connection.CreateCommand();
            cmd.CommandText = $"""
                               SELECT *
                               FROM {_embeddingRecordTableName}_{operationPathId}
                               WHERE
                                 document_id = @t1 AND
                                 index = ANY(@t2)
                               """;
            cmd.Parameters.AddWithValue("@t1", documentId);
            cmd.Parameters.AddWithValue("@t2", parentIndexes.ToArray());

            await using var dataReader = await cmd.ExecuteReaderAsync(cancellationToken);
            while (await dataReader.ReadAsync(cancellationToken))
            {
                var text = dataReader.GetString(dataReader.GetOrdinal("text"));
                var metadata = new Metadata
                {
                    DocumentId = dataReader.GetGuid(dataReader.GetOrdinal("document_id")),
                    OperationTreeId = dataReader.GetGuid(dataReader.GetOrdinal("operation_tree_id")),
                    Index = dataReader.GetInt32(dataReader.GetOrdinal("index")),
                    ParentIndex = null,
                    ParentChunk = null
                };
                var searchResult = new SearchResult(text, null, metadata);
                searchResults.Add(searchResult);
            }
        }

        return searchResults;
    }

    public async Task<IEnumerable<SearchResult>> GetNearbyChunksAsync(
        Guid documentId,
        int index,
        int range,
        int operationPathId,
        CancellationToken cancellationToken
    )
    {
        var searchResults = new List<SearchResult>();
        var connection = await _npgsql.OpenConnectionAsync(cancellationToken);

        await using (connection)
        {
            await using var cmd = connection.CreateCommand();
            cmd.CommandText = $"""
                               SELECT text, document_id, index, parent_index, parent_chunk, operation_tree_id
                               FROM {_embeddingRecordTableName}_{operationPathId}
                               WHERE
                                 document_id = @t1 AND
                                 ABS(index - @t2) BETWEEN 1 AND @t3
                               """;
            cmd.Parameters.AddWithValue("@t1", documentId);
            cmd.Parameters.AddWithValue("@t2", index);
            cmd.Parameters.AddWithValue("@t3", range);

            await using var dataReader = await cmd.ExecuteReaderAsync(cancellationToken);

            while (await dataReader.ReadAsync(cancellationToken))
            {
                var text = dataReader.GetString(dataReader.GetOrdinal("text"));
                var metadata = new Metadata
                {
                    DocumentId = dataReader.GetGuid(dataReader.GetOrdinal("document_id")),
                    OperationTreeId = dataReader.GetGuid(dataReader.GetOrdinal("operation_tree_id")),
                    Index = dataReader.GetInt32(dataReader.GetOrdinal("index")),
                    ParentIndex = null,
                    ParentChunk = null
                };
                var searchResult = new SearchResult(text, null, metadata);
                searchResults.Add(searchResult);
            }
        }

        return searchResults;
    }

    public async Task<int> DeleteByOperationTreeIdAsync(
        Guid operationTreeId,
        CancellationToken cancellationToken = default
    )
    {
        var connection = await _npgsql.OpenConnectionAsync(cancellationToken);

        await using (connection)
        {
            await using var cmd = connection.CreateCommand();
            cmd.CommandText = $"""
                               DELETE
                               FROM {_embeddingRecordTableName}
                               WHERE operation_tree_id = @t1
                               """;

            cmd.Parameters.AddWithValue("@t1", NpgsqlDbType.Uuid, operationTreeId);

            var deletedRows = await cmd.ExecuteNonQueryAsync(cancellationToken);

            return deletedRows;
        }
    }

    private async Task<IEnumerable<(Guid documentId, int index, string text)>> GetBatchByLikePatternAsync(
        string operationPath,
        int batchSize,
        int offset,
        CancellationToken cancellationToken = default
    )
    {
        var items = new List<(Guid documentId, int index, string text)>();
        var connection = await _npgsql.OpenConnectionAsync(cancellationToken);

        await using (connection)
        {
            await using var cmd = connection.CreateCommand();
            cmd.CommandText = $"""
                               SELECT
                                   document_id, index, text
                               FROM
                                   {_embeddingRecordTableName}
                               WHERE
                                   operations = @t1
                               LIMIT
                                   {batchSize}
                               OFFSET
                                   {offset}
                               """;
            cmd.Parameters.AddWithValue("@t1", operationPath);

            await using var dataReader = await cmd.ExecuteReaderAsync(cancellationToken);

            while (await dataReader.ReadAsync(cancellationToken))
            {
                var documentId = dataReader.GetGuid(dataReader.GetOrdinal("document_id"));
                var index = dataReader.GetInt32(dataReader.GetOrdinal("index"));
                var text = dataReader.GetString(dataReader.GetOrdinal("text"));
                items.Add((documentId, index, text));
            }
        }

        return items;
    }
}