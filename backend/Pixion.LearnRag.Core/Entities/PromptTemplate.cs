namespace Pixion.LearnRag.Core.Entities;

public class PromptTemplate(string templateString)
{
    public string TemplateString { get; } = templateString;
}