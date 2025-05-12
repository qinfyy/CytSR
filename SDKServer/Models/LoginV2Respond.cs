using System.Text.Json.Serialization;

namespace SDKServer.Models
{
    public class LoginV2Respond
    {
        [JsonPropertyName("retcode")]
        public int retcode { get; set; }

        [JsonPropertyName("message")]
        public string message { get; set; } = "";

        [JsonPropertyName("data")]
        public Data data { get; set; } = new Data();

        public class Data
        {
            [JsonPropertyName("account_type")]
            public int account_type { get; set; }

            [JsonPropertyName("heartbeat")]
            public bool? heartbeat { get; set; } = false;

            [JsonPropertyName("combo_id")]
            public string combo_id { get; set; } = "";

            [JsonPropertyName("combo_token")]
            public string combo_token { get; set; } = "";

            [JsonPropertyName("open_id")]
            public string open_id { get; set; } = "";

            [JsonPropertyName("data")]
            public string data { get; set; } = "";

            [JsonPropertyName("fatigue_remind")]
            public string fatigue_remind { get; set; } = "";
        }
    }
}
