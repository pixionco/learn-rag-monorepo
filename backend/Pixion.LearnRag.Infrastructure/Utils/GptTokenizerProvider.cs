using Microsoft.DeepDev;
using Pixion.LearnRag.Infrastructure.Interfaces;

namespace Pixion.LearnRag.Infrastructure.Utils;

public class GptTokenizerProvider : ITokenizerProvider
{
    private ITokenizer? _tokenizer;

    public ITokenizer GetTokenizer(string encoderName)
    {
        return _tokenizer ??= Task.Run(async () => await TokenizerBuilder.CreateByEncoderNameAsync(encoderName))
            .Result;
    }
}