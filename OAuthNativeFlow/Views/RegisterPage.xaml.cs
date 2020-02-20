using IpeluqueroPhoneApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace IpeluqueroPhoneApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RegisterPage : ContentPage
    {
        public RegisterPage()
        {
            InitializeComponent();
        }
        async void RegisterClicked(object sender, EventArgs e)
        {
            ApiService client = new ApiService();
            var getLoginDetails = await client.CheckLoginIfExists(Email.Text, Pass.Text);

            if (First.Text == null && Last.Text == null && Email.Text == null && Pass.Text == null && Confirm.Text == null)
            {
                await DisplayAlert("Registration Failed", "Enter your details", "Okay", "Cancel");
            }
            else if (getLoginDetails)
            {
                await DisplayAlert("Registration Failed", " Already have an account", "Okay", "Cancel");
            }
            else
            {
                await DisplayAlert("Registration successful", "Thanks for using our app", "Okay", "Cancel");
            }
        }
    }
}