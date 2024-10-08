using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Pixion.LearnRag.Infrastructure.Interfaces;

namespace Pixion.LearnRag.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static async Task SeedDatabase(this IServiceCollection services)
    {
        await using var serviceProvider = services.BuildServiceProvider();

        foreach (var service in serviceProvider.GetServices<ISeeder>())
            try
            {
                await service.SeedAsync();
            }
            catch (Exception ex)
            {
                var logger = serviceProvider.GetRequiredService<ILogger<ISeeder>>();
                logger.LogError(ex, "An error occurred seeding the DB. {ExceptionMessage}", ex.Message);
            }
    }
}