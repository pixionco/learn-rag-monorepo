using FluentValidation;

namespace Pixion.LearnRag.Infrastructure.Configs;

public class PostgresVectorRepositoryConfigValidator : AbstractValidator<PostgresVectorRepositoryConfig>
{
    public PostgresVectorRepositoryConfigValidator()
    {
        RuleFor(x => x.Schema)
            .NotEmpty()
            .WithMessage($"{nameof(PostgresVectorRepositoryConfig.Schema)} is required!");

        RuleFor(x => x.DatabaseConnection)
            .NotEmpty()
            .WithMessage($"{nameof(PostgresVectorRepositoryConfig.DatabaseConnection)} is required!");

        RuleFor(x => x.VectorSize)
            .NotEmpty()
            .WithMessage($"{nameof(PostgresVectorRepositoryConfig.VectorSize)} is required!");
    }
}