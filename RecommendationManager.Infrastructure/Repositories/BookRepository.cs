using Dapper;
using RecommendationManager.Application.Exceptions;
using RecommendationManager.Application.Interfaces;
using RecommendationManager.Domain;
using RecommendationManager.Infrastructure.Constants;
using RecommendationManager.Infrastructure.Database;

namespace RecommendationManager.Infrastructure.Repositories;

public class BookRepository : IBookRepository
{
    private readonly DataContext _context;

    public BookRepository(DataContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Book>> GetAll()
    {
        using var connection = _context.CreateConnection();
        const string sql = $"SELECT * FROM {DatabaseConstants.BooksTable}";

        return await connection.QueryAsync<Book>(sql);
    }

    public async Task<Book> GetById(string id)
    {
        using var connection = _context.CreateConnection();
        const string sql = $"""
                                SELECT * FROM {DatabaseConstants.BooksTable}
                                WHERE Id = @id
                            """;
        return await connection.QuerySingleOrDefaultAsync<Book>(
                   sql,
                   new { id }) ??
               throw new AppException("book not found");
    }

    public async Task<Book> SearchByTitle(string title)
    {
        using var connection = _context.CreateConnection();
        var sql = $"""
                       SELECT b.* FROM {DatabaseConstants.BooksTable} AS b
                       INNER JOIN {DatabaseConstants.BooksQueryTable} AS bq
                           ON bq.Id = b.Id
                       WHERE bq.Title MATCH @Title;
                   """;

        return await connection.QueryFirstOrDefaultAsync<Book>(
                   sql,
                   new { Title = title }) ??
               throw new AppException("book not found");
    }

    public async Task Create(Book book)
    {
        using var connection = _context.CreateConnection();
        const string sql = $"""
                                INSERT INTO {DatabaseConstants.BooksTable} (
                                    Id,
                                    Title,
                                    Edition,
                                    Source)
                                VALUES (@Id, @Title, @Edition, @Source);
                            
                                INSERT INTO {DatabaseConstants.BooksQueryTable} (
                                    Title,
                                    Id)
                                VALUES (@Title, @Id);
                            """;
        await connection.ExecuteAsync(sql, book);
    }

    public async Task Update(Book book)
    {
        using var connection = _context.CreateConnection();
        const string sql = $"""
                                UPDATE {DatabaseConstants.BooksTable}
                                SET Title = @Title,
                                    Edition = @Edition,
                                    Source = @Source
                                WHERE Id = @Id;
                            
                                UPDATE {DatabaseConstants.BooksQueryTable}
                                SET Title = @Title
                                WHERE Id = @Id;
                            """;
        await connection.ExecuteAsync(sql, book);
    }

    public async Task Delete(string id)
    {
        using var connection = _context.CreateConnection();
        const string sql = $"""
                                DELETE FROM {DatabaseConstants.BooksTable}
                                WHERE Id = @id;
                            
                                DELETE FROM {DatabaseConstants.BooksQueryTable}
                                WHERE Id = @id;
                            """;
        await connection.ExecuteAsync(sql, new { id });
    }
}