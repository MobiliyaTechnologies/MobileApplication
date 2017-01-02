using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Net.Http;
using System.ComponentModel;

namespace CSU_APP
{
    public partial class MS_POCPage : ContentPage, INotifyPropertyChanged
    {
        private bool isLoading;
        public bool IsLoading
        {
            get
            {
                return this.isLoading;
            }

            set
            {
                this.isLoading = value;
                RaisePropertyChanged("IsLoading");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string propName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }

        public MS_POCPage()
        {
            
            AuthenticationManager manager = AuthenticationManager.Instance;
            InitializeComponent();
            IsLoading = false;
            BindingContext = this;

            this.Title = "Login";

            performLogin.Clicked += async (sender, e) => {
                string userName = Uname.Text;
                string password = pword.Text;

                IsLoading = true;

                Models.UserDetails responseObj = await manager.performInternalLogin(userName, password);
                if (responseObj != null)
                {
                    var preferenceHandler = DependencyService.Get<IPreferencesHandler>();
                    preferenceHandler.SaveUserDetails(responseObj);
                    //await Application.Current.MainPage.Navigation.PushAsync(new DashboardPage());
                    await Application.Current.MainPage.Navigation.PushAsync(new MapPage());
                    IsLoading =  false;
                }
                else
                {
                    await DisplayAlert("Error", "Error while login", "OK");
                    //MessagingCenter.Send<Page>(this, "Error while login");
                    IsLoading = false;
                }
            };

            performForgotPassword.Clicked += async (sender, e) => {
                await Application.Current.MainPage.Navigation.PushAsync(new ForgotPassword());
                
            };
        }
    }
}
