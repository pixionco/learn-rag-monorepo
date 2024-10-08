using Pixion.LearnRag.Core.Clients;
using Pixion.LearnRag.Core.Entities;

namespace Pixion.LearnRag.Infrastructure.Clients;

public class PromptTemplateClient : IPromptTemplateClient
{
    private const string QuestionPromptTemplate =
        """
        We are creating an app, and you are a professor of law.
        Your task is to generate SPECIFIED number of DOMAIN RELATED questions that the USER MAY ASK.
        The questions should be generated based only on the provided <text> and not prior knowledge.
        The questions should not be connected with each other.
        The answers for generated questions must be contained within given <text>.
        If you can't generate any meaningful question, return a JSON with empty list.

        Here are the examples of a bad question, a reason why it is bad and a good question where its fixed.

        Example 1:
        BAD: What does the 'uniformed service' refer to in this text?
        REASON: Don't use phrases like "refer to in this text".
        GOOD: What is the uniformed service?

        Example 2:
        BAD: When should vessels observe the regulations prescribed by the Surgeon General from the provisions of subsections (a) and (b) of this section?
        REASON: Don't use phrases like "subsections (a) and (b) of this section".
        GOOD: When should vessels observe the regulations prescribed by the Surgeon General?

        <banned_phrases>:
        - in this section
        - in this text
        - in this context
        - in given text

        AVOID using <banned_phrases> and other phrases similar to them in your questions, i.e. imagine like you are creating questions for an exam and students DON'T have an access to the materials.

        You must generate 0-{{$numberOfQuestions}} questions!

        The output should be a well-formatted JSON object that conforms to the example below:
        {"questions": []}
        where "questions" is a list containing 0-{{$numberOfQuestions}} questions without <banned_phrases>.

        <text_start>
        {{$text}}
        <text_end>
        """;

    private const string SummaryPromptTemplate =
        """
        You are an assistant trained in creating summaries.
        When writing the summary, try to include important keywords.
        Create a summary of given <text>.
        Just summarize the <text>, don't write phrases like "The text contains..."!

        The output should be a well-formatted JSON object that conforms to the example below:
        {"summary": "some summary"}

        <text_start>
        {{$text}}
        <text_end>
        """;

    private const string NumberOfQuestionsKey = "numberOfQuestions";
    private const string TextKey = "text";

    public QuestionPromptTemplate GetQuestionPromptTemplate()
    {
        return new QuestionPromptTemplate(
            QuestionPromptTemplate,
            NumberOfQuestionsKey,
            TextKey
        );
    }

    public SummaryPromptTemplate GetSummaryPromptTemplate()
    {
        return new SummaryPromptTemplate(
            SummaryPromptTemplate,
            TextKey
        );
    }
}