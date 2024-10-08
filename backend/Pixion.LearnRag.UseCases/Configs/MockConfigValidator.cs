using FluentValidation;

namespace Pixion.LearnRag.UseCases.Configs;

public class MockConfigValidator : AbstractValidator<MockConfig>
{
    public MockConfigValidator()
    {
        RuleFor(x => x.MockAiModels)
            .NotNull()
            .WithMessage($"{nameof(MockConfig.MockAiModels)} is required!");
        RuleFor(x => x.UseFailingMocks)
            .NotNull()
            .WithMessage($"{nameof(MockConfig.UseFailingMocks)} is required!");
    }
}