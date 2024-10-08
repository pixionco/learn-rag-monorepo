using Ardalis.SmartEnum;

namespace Pixion.LearnRag.Core.Enums;

public sealed class ChunkSize : SmartEnum<ChunkSize, int>
{
    public static readonly ChunkSize C128 = new("ChunkSize128", 128);

    public static readonly ChunkSize C256 = new("ChunkSize256", 256);

    //public static readonly ChunkSize C384 = new("ChunkSize384", 384);
    public static readonly ChunkSize C512 = new("ChunkSize512", 512);

    //public static readonly ChunkSize C768 = new("ChunkSize768", 768);
    public static readonly ChunkSize C1024 = new("ChunkSize1024", 1024);
    public static readonly ChunkSize C2048 = new("ChunkSize2048", 2048);

    private ChunkSize(string name, int value) : base(name, value)
    {
    }
}