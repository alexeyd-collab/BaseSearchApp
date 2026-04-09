using System.Text.Json.Serialization;

namespace SearchApp.Models
{
    public class RepoOwner
    {
        [JsonPropertyName("login")]
        public string Name { get; set; }

        [JsonPropertyName("avatar_url")]
        public string AvatarUrl { get; set; }
    }
}
