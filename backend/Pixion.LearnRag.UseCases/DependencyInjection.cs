using System.Reflection;
using Ardalis.SharedKernel;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pixion.LearnRag.Core.Services;
using Pixion.LearnRag.Core.Services.RetrievalStrategy;
using Pixion.LearnRag.UseCases.Configs;
using Pixion.LearnRag.UseCases.MockServices;
using Pixion.LearnRag.UseCases.Services;
using Pixion.LearnRag.UseCases.Services.RetrievalStrategy;

namespace Pixion.LearnRag.UseCases;

public static class DependencyInjection
{
    public static IServiceCollection AddUseCasesServices(
        this IServiceCollection services,
        IConfiguration configuration,
        MockConfig mockConfig
    )
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddScoped<IDomainEventDispatcher, MediatRDomainEventDispatcher>();

        // Services
        if (mockConfig.MockAiModels)
        {
            if (mockConfig.UseFailingMocks)
            {
                services.AddSingleton<ISummaryGenerationService, SummaryGenerationServiceFailingMock>();
                services.AddSingleton<IQuestionGenerationService, QuestionGenerationServiceFailingMock>();
            }
            else
            {
                services.AddSingleton<ISummaryGenerationService, SummaryGenerationServiceMock>();
                services.AddSingleton<IQuestionGenerationService, QuestionGenerationServiceMock>();
            }
        }
        else
        {
            services.AddSingleton<ISummaryGenerationService, SummaryGenerationService>();
            services.AddSingleton<IQuestionGenerationService, QuestionGenerationService>();
        }

        services.AddSingleton<IChunkingService, ChunkingService>();

        // Operation services
        services.AddSingleton<IBasicStrategyService, BasicStrategyService>();
        services.AddSingleton<ISentenceWindowStrategyService, SentenceWindowStrategyService>();
        services.AddSingleton<IAutoMergingStrategyService, AutoMergingStrategyService>();
        services.AddSingleton<IHierarchicalStrategyService, HierarchicalStrategyService>();
        services.AddSingleton<IHypotheticalQuestionStrategyService, HypotheticalQuestionStrategyService>();

        return services;
    }
}