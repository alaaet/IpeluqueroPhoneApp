using System;
using System.Linq;
using System.Diagnostics;
using Newtonsoft.Json;
using Xamarin.Forms;
using Xamarin.Auth;
using Xamarin.Essentials;

namespace OAuthNativeFlow
{
    public partial class OAuthNativeFlowPage : ContentPage
    {
        Account account;
		Constants Const;
        public OAuthNativeFlowPage()
        {
            InitializeComponent();
        }

        async void OnLoginClicked(object sender, EventArgs e)
        {
			Button btn = (Button)sender;
			string clientId = null;
			string redirectUri = null;
			// Set the authentication provider
			switch (btn.ClassId)
			{
				case "Google":
					Const = new GoogleConstants();
					break;
				case "Facebook":
					Const = new FacebookConstants();
					break;
				default:
					Const = new GoogleConstants();
					break;
			}

			switch (Device.RuntimePlatform)
			{
				case Device.iOS:
					clientId = Const.iOSClientId;
					redirectUri = Const.iOSRedirectUrl;
					break;

				case Device.Android:
					clientId = Const.AndroidClientId;
					redirectUri = Const.AndroidRedirectUrl;
					break;
			}
			string serializedAccount = await SecureStorage.GetAsync(Const.AppName);
			if (serializedAccount != null) { 
				account = JsonConvert.DeserializeObject<Account>(serializedAccount);
			}
			

            var authenticator = new OAuth2Authenticator(
				clientId,
				null,
				Const.Scope,
				new Uri(Const.AuthorizeUrl),
				new Uri(redirectUri),
				new Uri(Const.AccessTokenUrl),
				null,
				true);

			authenticator.Completed += OnAuthCompleted;
			authenticator.Error += OnAuthError;

			AuthenticationState.Authenticator = authenticator;

			var presenter = new Xamarin.Auth.Presenters.OAuthLoginPresenter();
			presenter.Login(authenticator);           
        }

		async void OnAuthCompleted(object sender, AuthenticatorCompletedEventArgs e)
		{
            string access_token= "";
            var authenticator = sender as OAuth2Authenticator;
			if (authenticator != null)
			{
				authenticator.Completed -= OnAuthCompleted;
				authenticator.Error -= OnAuthError;                
			}
            try
            {
                access_token = e.Account.Properties["access_token"];
            }
            catch{}
            User user = null;
			if (e.IsAuthenticated)
			{
				// If the user is authenticated, request their basic user data from Google
				// UserInfoUrl = https://www.googleapis.com/oauth2/v2/userinfo
				var request = new OAuth2Request("GET", new Uri(Const.UserInfoUrl), null, e.Account);
				var response = await request.GetResponseAsync();
				if (response != null)
				{
					// Deserialize the data and store it in the account store
					// The users email address will be used to identify data in SimpleDB
					string userJson = await response.GetResponseTextAsync();
					user = JsonConvert.DeserializeObject<User>(userJson);
                    user.AccessToken = access_token;
				}

				if (account != null)
				{
					SecureStorage.Remove(Const.AppName);
				}

                await SecureStorage.SetAsync(Const.AppName, JsonConvert.SerializeObject(e.Account) );
                await DisplayAlert("Email address", user.Email, "OK");
                await DisplayAlert("Access token", user.AccessToken, "OK");

            }
		}

		void OnAuthError(object sender, AuthenticatorErrorEventArgs e)
		{
			var authenticator = sender as OAuth2Authenticator;
			if (authenticator != null)
			{
				authenticator.Completed -= OnAuthCompleted;
				authenticator.Error -= OnAuthError;
			}

			Debug.WriteLine("Authentication error: " + e.Message);
		}
    }
}
