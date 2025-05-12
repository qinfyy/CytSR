using System.Text.Json.Serialization;

namespace SDKServer.Models
{
    public class LoginRespond
    {
        [JsonPropertyName("message")]
        public string message { get; set; } = "";

        [JsonPropertyName("retcode")]
        public int retcode { get; set; }

        [JsonPropertyName("data")]
        public VerifyData data { get; set; } =  new VerifyData();

        public class VerifyData
        {
            [JsonPropertyName("account")]
            public VerifyAccountData account { get; set; } = new VerifyAccountData();

            [JsonPropertyName("device_grant_required")]
            public bool device_grant_required { get; set; } = false;

            [JsonPropertyName("realname_operation")]
            public string realname_operation { get; set; } = "NONE";

            [JsonPropertyName("realperson_required")]
            public bool realperson_required { get; set; } = false;

            [JsonPropertyName("safe_mobile_required")]
            public bool safe_mobile_required { get; set; } = false;
        }

        public class VerifyAccountData
        {
            [JsonPropertyName("uid")]
            public string uid { get; set; } = "";

            [JsonPropertyName("name")]
            public string name { get; set; } = "";

            [JsonPropertyName("email")]
            public string email { get; set; } = "";

            [JsonPropertyName("mobile")]
            public string mobile { get; set; } = "";

            [JsonPropertyName("is_email_verify")]
            public string is_email_verify { get; set; } = "0";

            [JsonPropertyName("realname")]
            public string realname { get; set; } = "";

            [JsonPropertyName("identity_card")]
            public string identity_card { get; set; } = "";

            [JsonPropertyName("token")]
            public string token { get; set; } = "";

            [JsonPropertyName("safe_mobile")]
            public string safe_mobile { get; set; } = "";

            [JsonPropertyName("facebook_name")]
            public string facebook_name { get; set; } = "";

            [JsonPropertyName("twitter_name")]
            public string twitter_name { get; set; } = "";

            [JsonPropertyName("game_center_name")]
            public string game_center_name { get; set; } = "";

            [JsonPropertyName("google_name")]
            public string google_name { get; set; } = "";

            [JsonPropertyName("apple_name")]
            public string apple_name { get; set; } = "";

            [JsonPropertyName("sony_name")]
            public string sony_name { get; set; } = "";

            [JsonPropertyName("tap_name")]
            public string tap_name { get; set; } = "";

            [JsonPropertyName("country")]
            public string country { get; set; } = "US";

            [JsonPropertyName("reactivate_ticket")]
            public string reactivate_ticket { get; set; } = "";

            [JsonPropertyName("area_code")]
            public string area_code { get; set; } = "**";

            [JsonPropertyName("device_grant_ticket")]
            public string device_grant_ticket { get; set; } = "";
        }
    }
}
