using Pixion.LearnRag.Core.Entities;

namespace Pixion.LearnRag.Core.Clients;

public interface IPromptTemplateClient
{
    QuestionPromptTemplate GetQuestionPromptTemplate();
    SummaryPromptTemplate GetSummaryPromptTemplate();
}