using FluentValidation;

namespace Pixion.LearnRag.Infrastructure.Configs;

public class AzureOpenAiEmbeddingConfigValidator : AbstractValidator<AzureOpenAiEmbeddingConfig>
{
    public AzureOpenAiEmbeddingConfigValidator()
    {
        RuleFor(x => x.Endpoint)
            .NotEmpty()
            .WithMessage($"{nameof(AzureOpenAiEmbeddingConfig.Endpoint)} is required!");

        RuleFor(x => x.ApiKey)
            .NotEmpty()
            .WithMessage($"{nameof(AzureOpenAiEmbeddingConfig.ApiKey)} is required!");

        RuleFor(x => x.DeploymentName)
            .NotEmpty()
            .WithMessage($"{nameof(AzureOpenAiEmbeddingConfig.DeploymentName)} is required!");
    }
}