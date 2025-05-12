using System.Text.Json.Serialization;

namespace SDKServer.Models
{
    public class TokenLoginRequst
    {
        [JsonPropertyName("uid")]
        public string uid { get; set; } = "";

        [JsonPropertyName("token")]
        public string token { get; set; } = "";
    }
}
