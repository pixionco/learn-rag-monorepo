using Microsoft.Extensions.Options;
using Pixion.LearnRag.Core.Enums;
using Pixion.LearnRag.Core.Repositories;
using Pixion.LearnRag.Infrastructure.Configs;

namespace Pixion.LearnRag.Infrastructure.Repositories;

public class HypotheticalQuestionStrategyRepository(IOptions<PostgresVectorRepositoryConfig> config)
    : StrategyRepository(config, Strategy.HypotheticalQuestion.Name), IHypotheticalQuestionStrategyRepository;