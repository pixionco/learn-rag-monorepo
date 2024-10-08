using Microsoft.SemanticKernel;
using Pixion.LearnRag.Core.Clients;

namespace Pixion.LearnRag.UseCases.PromptRenderFilters;

public class PromptRenderFilter(ITokenizingClient tokenizingClient) : IPromptRenderFilter
{
    public int InputTokenCount { get; private set; }

    public async Task OnPromptRenderAsync(PromptRenderContext context, Func<PromptRenderContext, Task> next)
    {
        await next(context);

        InputTokenCount = tokenizingClient.GetLargeLanguageModelTokenCount(context.RenderedPrompt + "<|endofprompt|>");
    }
}