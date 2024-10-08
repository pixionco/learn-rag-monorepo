using Ardalis.SmartEnum;
using Pixion.LearnRag.Core.Enums;


/* prices per 1000 tokens
 * as found on:
 * https://azure.microsoft.com/en-us/pricing/details/cognitive-services/openai-service/
 */

public sealed class AiModelPrice : SmartEnum<AiModelPrice, decimal>
{
    public static readonly AiModelPrice GptInput = new(AiModel.Gpt35Turbo, 0.003m);
    public static readonly AiModelPrice GptOutput = new(AiModel.Gpt35Turbo, 0.004m);
    public static readonly AiModelPrice Ada = new(AiModel.Ada, 0.000093m);

    private AiModelPrice(AiModel name, decimal value) : base(name.Name, value)
    {
    }
}