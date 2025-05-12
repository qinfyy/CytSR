using System.Text.Json.Serialization;
namespace SDKServer.Models
{
    public class LoginRequest
    {
        [JsonPropertyName("account")]
        public string Account { get; set; } = "";

        [JsonPropertyName("password")]
        public string Password { get; set; } = "";

        [JsonPropertyName("is_crypto")]
        public bool IsCrypto { get; set; }
    }
}