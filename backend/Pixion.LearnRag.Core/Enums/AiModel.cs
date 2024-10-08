using Ardalis.SmartEnum;

namespace Pixion.LearnRag.Core.Enums;

public sealed class AiModel : SmartEnum<AiModel, string>
{
    public static readonly AiModel Gpt35Turbo = new("Gpt35Turbo", "cl100k_base");
    public static readonly AiModel Ada = new("Ada", "cl100k_base");
    public static readonly AiModel TextEmbedding3Large = new("TextEmbedding3Large", "cl100k_base");

    private AiModel(string name, string value) : base(name, value)
    {
    }
}