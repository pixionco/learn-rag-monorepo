using Microsoft.DeepDev;

namespace Pixion.LearnRag.Infrastructure.Interfaces;

public interface ITokenizerProvider
{
    ITokenizer GetTokenizer(string encoderName);
}