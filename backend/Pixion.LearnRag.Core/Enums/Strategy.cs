using Ardalis.SmartEnum;

namespace Pixion.LearnRag.Core.Enums;

public sealed class Strategy : SmartEnum<Strategy, int>
{
    public static readonly Strategy Basic = new("basic", 1);
    public static readonly Strategy Hierarchical = new("hierarchical", 2);
    public static readonly Strategy AutoMerging = new("auto_merging", 3);
    public static readonly Strategy SentenceWindow = new("sentence_window", 4);
    public static readonly Strategy HypotheticalQuestion = new("hypothetical_question", 5);

    private Strategy(string name, int value) : base(name, value)
    {
    }
}