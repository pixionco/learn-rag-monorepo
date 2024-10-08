using Ardalis.SmartEnum;

namespace Pixion.LearnRag.Core.Enums;

public sealed class TextType : SmartEnum<TextType, int>
{
    public static readonly TextType Raw = new("raw", 1);
    public static readonly TextType Summary = new("summary", 2);
    public static readonly TextType Question = new("question", 3);

    private TextType(string name, int value) : base(name, value)
    {
    }
}