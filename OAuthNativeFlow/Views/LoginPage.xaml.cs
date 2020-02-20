using System;
using System.Diagnostics;
using Newtonsoft.Json;
using Xamarin.Forms;
using Xamarin.Auth;
using Xamarin.Essentials;
using Xamarin.Forms.Xaml;
using OAuthNativeFlow;
using System.Net.Http;

namespace IpeluqueroPhoneApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
        }
        void OnLoginClicked(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            string clientId = null;
            string redirectUri = null;
            Uri accessTokenUrl = null;
            OAuth2Authenticator authenticator;
            // Set the authentication provider
            switch (btn.ClassId)
            {
                case "Google":
                    App.Const = new GoogleConstants();
                    break;
                case "Facebook":
                    App.Const = new FacebookConstants();
                    break;
                default:
                    App.Const = new GoogleConstants();
                    break;
            }

            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    clientId = App.Const.iOSClientId;
                    redirectUri = App.Const.iOSRedirectUrl;
                    break;

                case Device.Android:
                    clientId = App.Const.AndroidClientId;
                    redirectUri = App.Const.AndroidRedirectUrl;
                    break;
            }


            accessTokenUrl = App.Const.AccessTokenUrl != null ? new Uri(App.Const.AccessTokenUrl) : null;
            if (accessTokenUrl != null)
            {
                authenticator = new OAuth2Authenticator(
                clientId,
                null,
                App.Const.Scope,
                new Uri(App.Const.AuthorizeUrl),
                new Uri(redirectUri),
                accessTokenUrl,
                null,
                true);
            }
            else
            {
                authenticator = new OAuth2Authenticator(
                clientId,
                App.Const.Scope,
                new Uri(App.Const.AuthorizeUrl),
                new Uri(redirectUri));
            }


            authenticator.Completed += OnAuthCompleted;
            authenticator.Error += OnAuthError;

            AuthenticationState.Authenticator = authenticator;

            var presenter = new Xamarin.Auth.Presenters.OAuthLoginPresenter();
            presenter.Login(authenticator);
        }

        async void OnAuthCompleted(object sender, AuthenticatorCompletedEventArgs e)
        {
            string access_token = "";
            var authenticator = sender as OAuth2Authenticator;
            if (authenticator != null)
            {
                authenticator.Completed -= OnAuthCompleted;
                authenticator.Error -= OnAuthError;
            }
            try
            {
                App.User.AccessToken = access_token = e.Account.Properties["access_token"];
            }
            catch { }
            if (e.IsAuthenticated)
            {
                string userJson = "";
                switch (App.Const.GetType().Name)
                {
                    case "GoogleConstants":
                        // If the user is authenticated, request their basic user data from Google
                        // UserInfoUrl = https://www.googleapis.com/oauth2/v2/userinfo
                        var request = new OAuth2Request("GET", new Uri(App.Const.UserInfoUrl), null, e.Account);
                        var response = await request.GetResponseAsync();
                        if (response != null)
                        {
                            // Deserialize the data and store it in the account store
                            // The users email address will be used to identify data in SimpleDB
                            userJson = await response.GetResponseTextAsync();
                            App.User = JsonConvert.DeserializeObject<User>(userJson);
                        }
                        break;
                    case "FacebookConstants":
                        var httpClient = new HttpClient();
                        userJson = await httpClient.GetStringAsync(
                            $"https://graph.facebook.com/me?fields=name,picture.type(large),email,birthday&access_token={access_token}");
                        App.User = JsonConvert.DeserializeObject<User>(userJson);
                        break;
                    default:
                        await DisplayAlert("Error", "Something went wrong, contact helpdesk please.", "Ok");
                        break;
                }

                // Clean previously saved values
                if (App.User != null){SecureStorage.Remove(App.Const.AppName);}
                await SecureStorage.SetAsync(App.Const.AppName, JsonConvert.SerializeObject(App.User));
                await Navigation.PopModalAsync();
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