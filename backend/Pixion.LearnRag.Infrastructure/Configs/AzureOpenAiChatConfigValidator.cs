using FluentValidation;

namespace Pixion.LearnRag.Infrastructure.Configs;

public class AzureOpenAiChatConfigValidator : AbstractValidator<AzureOpenAiChatConfig>
{
    public AzureOpenAiChatConfigValidator()
    {
        RuleFor(x => x.Endpoint)
            .NotEmpty()
            .WithMessage($"{nameof(AzureOpenAiChatConfig.Endpoint)} is required!");

        RuleFor(x => x.ApiKey)
            .NotEmpty()
            .WithMessage($"{nameof(AzureOpenAiChatConfig.ApiKey)} is required!");

        RuleFor(x => x.DeploymentName)
            .NotEmpty()
            .WithMessage($"{nameof(AzureOpenAiChatConfig.DeploymentName)} is required!");
    }
}