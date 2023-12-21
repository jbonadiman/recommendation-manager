using System.Text.Json.Serialization;

namespace RecommendationManager.Application.Models.Books;

public class SaveBookRequest
{
    [JsonPropertyName("name")]
    public string? BundleName { get; set; }

    [JsonPropertyName("items")]
    public IEnumerable<string>? Items { get; set; }
}
