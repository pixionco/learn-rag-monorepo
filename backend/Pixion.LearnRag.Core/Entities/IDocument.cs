namespace Pixion.LearnRag.Core.Entities;

public interface IDocument
{
    Guid GetId();

    string GetText();
}