namespace OAuthNativeFlow
{
    public static class Constants
    {
        public static string AppName = "OAuthNativeFlow";

		// OAuth
		// For Google login, configure at https://console.developers.google.com/
		public static string iOSClientId = "<insert IOS client ID here>";
		public static string AndroidClientId = "620512300606-pki8n4quk8jbco9242pq6j7ti6df1nh0.apps.googleusercontent.com";

		// These values do not need changing
		public static string Scope = "https://www.googleapis.com/auth/userinfo.profile https://www.googleapis.com/auth/userinfo.email";
		public static string AuthorizeUrl = "https://accounts.google.com/o/oauth2/auth";
		public static string AccessTokenUrl = "https://www.googleapis.com/oauth2/v4/token";
		public static string UserInfoUrl = "https://www.googleapis.com/oauth2/v2/userinfo";

		// Set these to reversed iOS/Android client ids, with :/oauth2redirect appended
		public static string iOSRedirectUrl = "";
        public static string AndroidRedirectUrl = "com.googleusercontent.apps.620512300606-pki8n4quk8jbco9242pq6j7ti6df1nh0:/oauth2redirect";
    }
}
