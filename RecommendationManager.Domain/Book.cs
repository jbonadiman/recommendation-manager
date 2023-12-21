namespace RecommendationManager.Domain;

public record Book
{
    public string? Id { get; set; }

    public string? Title { get; set; }

    public string? Edition { get; set; }

    public string? Source { get; set; }
}