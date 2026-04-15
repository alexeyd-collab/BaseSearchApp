using System.Text.Json.Serialization;

namespace SearchApp.Models
{
    public class RepoSearchItem
    {
        [JsonPropertyName("name")]
        public required string Name { get; set; }

        [JsonPropertyName("html_url")]
        public required string Url { get; set; }

        [JsonPropertyName("owner")]
        public required RepoOwner RepoOwner { get; set; }
    }
}
