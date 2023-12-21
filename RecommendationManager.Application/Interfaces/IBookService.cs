using RecommendationManager.Application.Models.Books;
using RecommendationManager.Domain;

namespace RecommendationManager.Application.Interfaces;

public interface IBookService
{
    Task SaveBundleAsync(SaveBundleRequest saveRequest);
    Task<IEnumerable<Book>> GetAllAsync();
}