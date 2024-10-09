using Microsoft.Extensions.Options;
using Pixion.LearnRag.Core.Entities;
using Pixion.LearnRag.Core.Enums;
using Pixion.LearnRag.Core.Models;
using Pixion.LearnRag.Core.Repositories;
using Pixion.LearnRag.Infrastructure.Configs;

namespace Pixion.LearnRag.Infrastructure.Repositories;

public class SentenceWindowStrategyRepository(IOptions<PostgresVectorRepositoryConfig> config)
    : StrategyRepository(config, Strategy.SentenceWindow.Name), ISentenceWindowStrategyRepository
{
    public async Task<IEnumerable<SearchResult>> GetNearbyChunksAsync(Guid documentId, int index, int range, CancellationToken cancellationToken = default)
    {
        var searchResults = new List<SearchResult>();
        var connection = await Npgsql.OpenConnectionAsync(cancellationToken);

        await using (connection)
        {
            await using var cmd = connection.CreateCommand();
            cmd.CommandText = $"""
                               SELECT text, metadata
                               FROM {Table}
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
                searchResults.Add(this.ReadSearchResult(dataReader));
            }
        }

        return searchResults;
    }
}