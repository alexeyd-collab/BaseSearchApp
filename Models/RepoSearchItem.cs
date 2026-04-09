using System.Text.Json.Serialization;

namespace SearchApp.Models
{
    public class RepoSearchItem
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("html_url")]
        public string Url { get; set; }

        [JsonPropertyName("owner")]
        public RepoOwner RepoOwner { get; set; }
    }
}
