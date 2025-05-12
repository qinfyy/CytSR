using System.Text.Json.Serialization;

namespace SDKServer.Models
{
    public class LoginV2Request
    {
        [JsonPropertyName("app_id")]
        public int app_id { get; set; }

        [JsonPropertyName("channel_id")]
        public int channel_id { get; set; }

        [JsonPropertyName("data")]
        public string data { get; set; } = "";

        [JsonPropertyName("device")]
        public string device { get; set; } = "";

        [JsonPropertyName("sign")]
        public string sign { get; set; } = "";

        public class Data
        {
            [JsonPropertyName("uid")]
            public string uid { get; set; } = "";

            [JsonPropertyName("token")]
            public string token { get; set; } = "";

            [JsonPropertyName("guest")]
            public bool guest { get; set; }
        }
    }
}

