using System.Text.Json.Serialization;

namespace RecommendationManager.Application.Models.Books;

public class SaveBundleRequest
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("items")]
    public IEnumerable<string>? Items { get; set; }
}
