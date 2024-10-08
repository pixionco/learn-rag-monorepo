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

public class SummaryGenerationService(
    Kernel kernel,
    IPromptTemplateClient promptTemplateClient,
    ITokenizingClient tokenizingClient
) : ISummaryGenerationService
{
    private readonly JsonSerializerOptions _options = new() { PropertyNameCaseInsensitive = true };

    public async Task<SummaryGenerationResult> GenerateSummaryAsync(string text)
    {
        return await Policy
            .HandleResult<SummaryGenerationResult>(
                result => result is SummaryGenerationErrorResult { StatusCode: HttpStatusCode.TooManyRequests }
            )
            .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)))
            .ExecuteAsync(async () => await GenerateSummaryHelperAsync(text));
    }

    private async Task<SummaryGenerationResult> GenerateSummaryHelperAsync(string text)
    {
        var promptTemplate = promptTemplateClient.GetSummaryPromptTemplate();
        var executionSettings = new OpenAIPromptExecutionSettings
        {
            Temperature = 0.1, ResponseFormat = ChatResponseFormat.JsonObject
        };
        var summaryGenerationFunction =
            kernel.CreateFunctionFromPrompt(promptTemplate.TemplateString, executionSettings);
        var arguments = new KernelArguments { { promptTemplate.TextKey, text } };

        var promptRenderFilter = new PromptRenderFilter(tokenizingClient);
        kernel.PromptRenderFilters.Add(promptRenderFilter);

        try
        {
            var res = await summaryGenerationFunction.InvokeAsync(kernel, arguments);
            SummaryGenerationResponse? summaryGenerationResponse;
            try
            {
                summaryGenerationResponse =
                    JsonSerializer.Deserialize<SummaryGenerationResponse>(res.ToString(), _options);
            }
            catch (Exception ex)
            {
                return new SummaryGenerationErrorResult(ex, promptRenderFilter.InputTokenCount, res.ToString(), null);
            }

            var summary = summaryGenerationResponse?.Summary ?? string.Empty;

            if (res.Metadata == null) throw new NullReferenceException("Metadata cannot be null!");

            if (res.Metadata["Usage"] is not ChatTokenUsage usage)
                throw new NullReferenceException("Metadata.Usage cannot be null!");

            return new SummaryGenerationSuccessResult(summary, usage.InputTokens, usage.OutputTokens);
        }
        catch (HttpOperationException ex)
        {
            return new SummaryGenerationErrorResult(
                ex,
                ex.StatusCode == HttpStatusCode.TooManyRequests ? 0 : promptRenderFilter.InputTokenCount,
                null,
                ex.StatusCode
            );
        }
        catch (Exception ex)
        {
            return new SummaryGenerationErrorResult(ex, promptRenderFilter.InputTokenCount, null, null);
        }
        finally
        {
            kernel.PromptRenderFilters.Remove(promptRenderFilter);
        }
    }

    private class SummaryGenerationResponse
    {
        public string Summary { get; init; } = string.Empty;
    }
}