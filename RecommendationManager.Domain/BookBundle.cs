namespace RecommendationManager.Domain;

public record Bundle
{
    public string? Name { get; set; }
    public IEnumerable<Book> Items { get; set; } = Enumerable.Empty<Book>();
}