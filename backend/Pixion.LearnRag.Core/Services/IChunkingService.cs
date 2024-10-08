using System.Runtime.InteropServices.JavaScript;

namespace Pixion.LearnRag.Core.Services;

public interface IChunkingService
{
    IEnumerable<string> ChunkText(string text, int chunkSize, int chunkOverlap);
}