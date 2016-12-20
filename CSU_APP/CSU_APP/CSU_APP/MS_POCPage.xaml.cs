using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Net.Http;

namespace CSU_APP
{
    public partial class MS_POCPage : ContentPage
    {
        public MS_POCPage()
        {
            AuthenticationManager manager = AuthenticationManager.Instance;
            InitializeComponent();
            this.Title = "Login";


            performLogin.Clicked += async (sender, e) => {
                IsBusy = true;
                string userName = Uname.Text;
                string password = pword.Text;

                AuthenticationManager.LoginResponse responseObj = await manager.performInternalLogin(userName, password);
                if (responseObj != null)
                {
                    await Application.Current.MainPage.Navigation.PushAsync(new DashboardPage());
                    IsBusy = false;
                }
                else
                {
                    await DisplayAlert("Error", "Error while login", "OK");
                    //MessagingCenter.Send<Page>(this, "Error while login");
                    IsBusy = false;
                }
            };
        }
    }
}
