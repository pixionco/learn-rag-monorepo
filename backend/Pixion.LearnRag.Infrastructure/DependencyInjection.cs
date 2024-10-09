using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Pixion.LearnRag.Core.Clients;
using Pixion.LearnRag.Core.Repositories;
using Pixion.LearnRag.Infrastructure.ClientMocks;
using Pixion.LearnRag.Infrastructure.Clients;
using Pixion.LearnRag.Infrastructure.Configs;
using Pixion.LearnRag.Infrastructure.Interfaces;
using Pixion.LearnRag.Infrastructure.Repositories;
using Pixion.LearnRag.Infrastructure.Seeders;
using Pixion.LearnRag.Infrastructure.Utils;
using Pixion.LearnRag.UseCases.Configs;

namespace Pixion.LearnRag.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        MockConfig mockConfig
    )
    {
        // Repositories
        services.AddSingleton<IBasicStrategyRepository, BasicStrategyRepository>();
        services.AddSingleton<ISentenceWindowStrategyRepository, SentenceWindowStrategyRepository>();
        services.AddSingleton<IAutoMergingStrategyRepository, AutoMergingStrategyRepository>();
        services.AddSingleton<IHierarchicalStrategyRepository, HierarchicalStrategyRepository>();
        services.AddSingleton<IHypotheticalQuestionStrategyRepository, HypotheticalQuestionStrategyRepository>();

        // Services
        if (mockConfig.MockAiModels)
        {
            if (mockConfig.UseFailingMocks)
                services.AddSingleton<IEmbeddingClient, AzureOpenAiEmbeddingClientFailingMock>();
            else
                services.AddSingleton<IEmbeddingClient, AzureOpenAiEmbeddingClientMock>();
        }
        else
        {
            services.AddSingleton<IEmbeddingClient, AzureOpenAiEmbeddingClient>();
        }

        // Semantic Kernel 
        using var servicesProvider = services.BuildServiceProvider();
        var azureOpenAiChatConfig = servicesProvider.GetService<IOptions<AzureOpenAiChatConfig>>()
            ?.Value;
        ArgumentNullException.ThrowIfNull(azureOpenAiChatConfig);
        services
            .AddKernel()
            .AddAzureOpenAIChatCompletion(
                azureOpenAiChatConfig.DeploymentName,
                azureOpenAiChatConfig.Endpoint,
                azureOpenAiChatConfig.ApiKey
            );

        // Postgres Seeders
        services.AddScoped<ISeeder, StrategyTableSeeder>();

        // Clients
        services.AddTransient<ITokenizingClient, TokenizingClient>();
        services.AddTransient<IPromptTemplateClient, PromptTemplateClient>();

        // Factories
        services.AddTransient<ITokenizerProvider, GptTokenizerProvider>();

        return services;
    }
}