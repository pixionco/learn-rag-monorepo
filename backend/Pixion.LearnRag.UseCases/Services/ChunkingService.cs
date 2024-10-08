using Microsoft.SemanticKernel.Text;
using Pixion.LearnRag.Core.Clients;
using Pixion.LearnRag.Core.Services;

namespace Pixion.LearnRag.UseCases.Services;

#pragma warning disable SKEXP0050
#pragma warning disable SKEXP0055
public class ChunkingService(ITokenizingClient tokenizingClient) : IChunkingService
{
    public IEnumerable<string> ChunkText(
        string text,
        int size,
        int overlap
    )
    {
        return TextChunker.SplitPlainTextParagraphs(
            text.Split(["\n"], StringSplitOptions.None),
            size,
            (int)Math.Round(size * overlap / 100.0),
            tokenCounter: tokenizingClient.GetEmbeddingModelTokenCount
        );
    }
}