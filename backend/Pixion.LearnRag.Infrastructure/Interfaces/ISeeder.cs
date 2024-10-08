namespace Pixion.LearnRag.Infrastructure.Interfaces;

public interface ISeeder
{
    public Task SeedAsync(CancellationToken cancellationToken = default);
}