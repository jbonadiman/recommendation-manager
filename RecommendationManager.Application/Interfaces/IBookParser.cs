using RecommendationManager.Domain;

namespace RecommendationManager.Application.Interfaces;

public interface IBookParser
{
    IAsyncEnumerable<Book> ParseBundle(BookBundle bundle);
}
