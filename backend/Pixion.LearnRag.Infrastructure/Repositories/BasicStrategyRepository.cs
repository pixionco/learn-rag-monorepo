using Microsoft.Extensions.Options;
using Pixion.LearnRag.Core.Enums;
using Pixion.LearnRag.Core.Repositories;
using Pixion.LearnRag.Infrastructure.Configs;

namespace Pixion.LearnRag.Infrastructure.Repositories;

public class BasicStrategyRepository(IOptions<PostgresVectorRepositoryConfig> config)
    : StrategyRepository(config, Strategy.Basic.Name), IBasicStrategyRepository;