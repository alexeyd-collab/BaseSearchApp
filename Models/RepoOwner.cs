using System.Text.Json.Serialization;

namespace SearchApp.Models
{
    public class RepoOwner
    {
        [JsonPropertyName("login")]
        public required string Name { get; set; }

        [JsonPropertyName("avatar_url")]
        public required string AvatarUrl { get; set; }
    }
}
