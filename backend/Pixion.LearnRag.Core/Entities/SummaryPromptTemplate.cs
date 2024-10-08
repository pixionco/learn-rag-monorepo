namespace Pixion.LearnRag.Core.Entities;

public class SummaryPromptTemplate(
    string templateString,
    string textKey
) : PromptTemplate(templateString)
{
    public string TextKey { get; } = textKey;
}