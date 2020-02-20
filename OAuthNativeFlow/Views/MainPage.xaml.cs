using IpeluqueroPhoneApp.Views;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace OAuthNativeFlow.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {

            InitializeComponent();
            initAuth();
        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (App.User != null)
            {
                lblEmail.Text = App.User.Email != null ? App.User.Email : "";
                lblName.Text = App.User.Name != null ? App.User.Name : "";
                lblToken.Text = App.User.AccessToken != null ? App.User.AccessToken : "";
                lblPicture.Text = App.User.Picture != null ? App.User.Picture : "";
                picture.Source = App.User.Picture != null ? App.User.Picture : "";
            }
            else
            {
                await Navigation.PushModalAsync(new LoginPage());
            }


        }
        async void initAuth()
        {

            try
            {
                string serializedAccount = await SecureStorage.GetAsync(App.Const.AppName);
                if (serializedAccount != null)
                {
                    App.User = JsonConvert.DeserializeObject<User>(serializedAccount);
                }
            }
            catch (Exception)
            {
                // Go to login page
                await Navigation.PushModalAsync(new LoginPage());
            }
        }

        async void OnLogoutClicked(object sender, EventArgs e)
        {
            if (App.User != null)
            {
                SecureStorage.Remove(App.Const.AppName);
                App.User = null;
            }
            await Navigation.PushModalAsync(new LoginPage());
        }




    }
}