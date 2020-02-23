namespace OAuthNativeFlow
{
	public class Constants
	{
		public string AppName = "OAuthNativeFlow";
        public string iOSClientId, AndroidClientId, Scope, AuthorizeUrl, AccessTokenUrl, UserInfoUrl, iOSRedirectUrl, AndroidRedirectUrl;

		// API CONSTANTS:
		public string APIBaseUrl = "https://todo-demo-alaa.herokuapp.com/api/";
		public string APINativeLoginRealtivePath = "auth/login";
		public string APISocialLoginRealtivePath = "auth/social_login";
		public string DataTypeJson = "application/json";
		// FB Graph constants:
		public string FBBaseUrl = "https://graph.facebook.com/me";
	}
	public class GoogleConstants: Constants
	{
		public GoogleConstants() {
			// Google constants
			// For Google login, configure at https://console.developers.google.com/
			iOSClientId = "<insert IOS client ID here>";
			AndroidClientId = "620512300606-pki8n4quk8jbco9242pq6j7ti6df1nh0.apps.googleusercontent.com";
			Scope = "https://www.googleapis.com/auth/userinfo.profile https://www.googleapis.com/auth/userinfo.email";
			AuthorizeUrl = "https://accounts.google.com/o/oauth2/auth";
			AccessTokenUrl = "https://www.googleapis.com/oauth2/v4/token";
			UserInfoUrl = "https://www.googleapis.com/oauth2/v2/userinfo";
			iOSRedirectUrl = "";
			AndroidRedirectUrl = "com.googleusercontent.apps.620512300606-pki8n4quk8jbco9242pq6j7ti6df1nh0:/oauth2redirect";
		}
	}

	public class FacebookConstants: Constants
	{
		public FacebookConstants()
		{
			// Facebook constants
			// For Facebook login, configure at https://developers.facebook.com/apps
			iOSClientId = "610327619817321";
			AndroidClientId = "610327619817321";
			Scope = "email public_profile user_birthday";
			AuthorizeUrl = "https://www.facebook.com/dialog/oauth/";
			//AccessTokenUrl = "";
			//UserInfoUrl = "";
			iOSRedirectUrl = "https://www.facebook.com/connect/login_success.html";
			AndroidRedirectUrl = "https://www.facebook.com/connect/login_success.html";
		}
	}
}
