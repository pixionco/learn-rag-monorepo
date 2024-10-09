using Microsoft.Extensions.Options;
using Pixion.LearnRag.Core.Enums;
using Pixion.LearnRag.Core.Models;
using Pixion.LearnRag.Core.Repositories;
using Pixion.LearnRag.Infrastructure.Configs;

namespace Pixion.LearnRag.Infrastructure.Repositories;

public class AutoMergingStrategyRepository(IOptions<PostgresVectorRepositoryConfig> config)
    : StrategyRepository(config, Strategy.AutoMerging.Name), IAutoMergingStrategyRepository
{
    public async Task<IEnumerable<SearchResult>> GetParentsAsync(Guid documentId, IEnumerable<int> parentIndexes,
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
}