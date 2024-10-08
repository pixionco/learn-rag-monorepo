using Ardalis.SmartEnum;

namespace Pixion.LearnRag.Core.Enums;

public sealed class ChunkOverlap : SmartEnum<ChunkOverlap, int>
{
    public static readonly ChunkOverlap O0 = new("ChunkOverlap0", 0);
    public static readonly ChunkOverlap O10 = new("ChunkOverlap10", 10);
    public static readonly ChunkOverlap O20 = new("ChunkOverlap20", 20);
    public static readonly ChunkOverlap O30 = new("ChunkOverlap30", 30);
    public static readonly ChunkOverlap O40 = new("ChunkOverlap40", 40);
    public static readonly ChunkOverlap O50 = new("ChunkOverlap50", 50);

    private ChunkOverlap(string name, int value) : base(name, value)
    {
    }
}