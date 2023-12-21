using RecommendationManager.Domain;

namespace RecommendationManager.Application.Interfaces;

public interface IBookRepository
{
    Task<IEnumerable<Book>> GetAll();
    Task<Book> GetById(string id);
    Task<Book> SearchByTitle(string title);
    Task Create(Book book);
    Task Update(Book book);
    Task Delete(string id);
}