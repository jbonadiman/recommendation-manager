namespace RecommendationManager.Domain;

public record BookBundle
{
    public string? Name { get; set; }
    public IEnumerable<string> Items { get; set; } = Enumerable.Empty<string>();
}
