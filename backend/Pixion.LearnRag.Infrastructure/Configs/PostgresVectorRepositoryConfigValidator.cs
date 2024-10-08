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

        RuleFor(x => x.VectorSize)
            .NotEmpty()
            .WithMessage($"{nameof(PostgresVectorRepositoryConfig.EmbeddingRecordTableName)} is required!");

        RuleFor(x => x.AiModelFailedMessageTableName)
            .NotEmpty()
            .WithMessage($"{nameof(PostgresVectorRepositoryConfig.AiModelFailedMessageTableName)} is required!");
        RuleFor(x => x.OperationPathTableName)
            .NotEmpty()
            .WithMessage($"{nameof(PostgresVectorRepositoryConfig.OperationPathTableName)} is required!");
    }
}