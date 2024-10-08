using System.Net;
using System.Text.Json;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using OpenAI.Chat;
using Pixion.LearnRag.Core.Clients;
using Pixion.LearnRag.Core.Models;
using Pixion.LearnRag.Core.Services;
using Pixion.LearnRag.UseCases.PromptRenderFilters;
using Polly;

namespace Pixion.LearnRag.UseCases.Services;

#pragma warning disable SKEXP0001
#pragma warning disable SKEXP0010
#pragma warning disable SKEXP0013

public class QuestionGenerationService(
    Kernel kernel,
    IPromptTemplateClient promptTemplateClient,
    ITokenizingClient tokenizingClient
) : IQuestionGenerationService
{
    private readonly JsonSerializerOptions _options = new() { PropertyNameCaseInsensitive = true };

    public async Task<QuestionGenerationResult> GenerateQuestionsAsync(string text, int numberOfQuestions)
    {
        return await Policy
            .HandleResult<QuestionGenerationResult>(
                result => result is QuestionGenerationErrorResult { StatusCode: HttpStatusCode.TooManyRequests }
            )
            .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)))
            .ExecuteAsync(async () => await GenerateQuestionsHelperAsync(text, numberOfQuestions));
    }

    private async Task<QuestionGenerationResult> GenerateQuestionsHelperAsync(string text, int numberOfQuestions)
    {
        var promptTemplate = promptTemplateClient.GetQuestionPromptTemplate();
        var executionSettings = new OpenAIPromptExecutionSettings
        {
            Temperature = 0.1, ResponseFormat = ChatResponseFormat.JsonObject
        };
        var questionGenerationFunction =
            kernel.CreateFunctionFromPrompt(promptTemplate.TemplateString, executionSettings);

        var arguments = new KernelArguments
        {
            { promptTemplate.NumberOfQuestionsKey, numberOfQuestions }, { promptTemplate.TextKey, text }
        };

        var promptRenderFilter = new PromptRenderFilter(tokenizingClient);
        kernel.PromptRenderFilters.Add(promptRenderFilter);
        try
        {
            var res = await questionGenerationFunction.InvokeAsync(kernel, arguments);
            QuestionGenerationResponse? questionGenerationResponse;
            try
            {
                questionGenerationResponse =
                    JsonSerializer.Deserialize<QuestionGenerationResponse>(res.ToString(), _options);
            }
            catch (Exception ex)
            {
                return new QuestionGenerationErrorResult(
                    ex,
                    promptRenderFilter.InputTokenCount,
                    res.ToString(),
                    null
                );
            }

            var questions = questionGenerationResponse?.Questions.Take(numberOfQuestions) ?? [];

            if (res.Metadata == null) throw new NullReferenceException("Metadata cannot be null!");

            if (res.Metadata["Usage"] is not ChatTokenUsage usage)
                throw new NullReferenceException("Metadata.Usage cannot be null!");

            return new QuestionGenerationSuccessResult(questions, usage.InputTokens, usage.OutputTokens);
        }
        catch (HttpOperationException ex)
        {
            return new QuestionGenerationErrorResult(
                ex,
                ex.StatusCode == HttpStatusCode.TooManyRequests ? 0 : promptRenderFilter.InputTokenCount,
                null,
                ex.StatusCode
            );
        }
        catch (Exception ex)
        {
            return new QuestionGenerationErrorResult(
                ex,
                promptRenderFilter.InputTokenCount,
                null,
                null
            );
        }
        finally
        {
            kernel.PromptRenderFilters.Remove(promptRenderFilter);
        }
    }

    private class QuestionGenerationResponse
    {
        public List<string> Questions { get; init; } = [];
    }
}