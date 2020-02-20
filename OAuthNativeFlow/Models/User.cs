using IpeluqueroPhoneApp.Helpers;
using Newtonsoft.Json;

namespace OAuthNativeFlow
{
	[JsonObject]
    public class User
	{
		[JsonProperty("id")]
		public string Id { get; set; }

        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("email")]
		public string Email { get; set; }

		[JsonProperty("verified_email")]
		public bool VerifiedEmail { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("given_name")]
		public string GivenName { get; set; }

		[JsonProperty("family_name")]
		public string FamilyName { get; set; }

		[JsonProperty("link")]
		public string Link { get; set; }

		[JsonProperty("picture")]
        [JsonConverter(typeof(UserDataConverter))]
        public string Picture { get; set; }

        [JsonProperty("gender")]
		public string Gender { get; set; }
        [JsonProperty("birthday")]
        public string Birthday { get; set; }
    }
}
