using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace CSU_APP
{
    public partial class SettingPage : ContentPage
    {
        public SettingPage()
        {
            settingViewModel vm;
            BindingContext = vm = new settingViewModel();
            InitializeComponent();
            this.Title = "Menu";
            vm.setDataForTableRow();
            SettingList.ItemTapped += async (object sender, ItemTappedEventArgs e) => {

                cellModel model = (cellModel)e.Item;
                if (model.title == "Logout")
                {
                    IsBusy = true;

                    var preferenceHandler = DependencyService.Get<IPreferencesHandler>();
                    string email = preferenceHandler.GetUserDetails().Email;
                    if (email != null && email != "")
                    {
                        AuthenticationManager manager = AuthenticationManager.Instance;
                        bool responseObj = await manager.performInternalLogOut(email);
                        if (responseObj)
                        {
                            IsBusy = false;
                            await Application.Current.MainPage.Navigation.PopToRootAsync();
                        }
                        else
                        {
                            IsBusy = false;
                            await DisplayAlert("Alert", "Failed to logout", "OK");
                        }
                    }
                }
                else if(model.title == "Reports")
                {
                    await Application.Current.MainPage.Navigation.PushAsync(new CommonReportsPage());
                }
                else
                {
                    await Application.Current.MainPage.Navigation.PushAsync(new ResetPassword());
                }
            };
        }
    }
}
