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
            this.Title = "Settings";
            vm.setDataForTableRow();
            SettingList.ItemTapped += async (object sender, ItemTappedEventArgs e) => {

                cellModel model = (cellModel)e.Item;
                if (model.title == "Logout")
                {
                    IsBusy = true;
                    AuthenticationManager manager = AuthenticationManager.Instance;
                    bool responseObj = await manager.performInternalLogOut();
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
                else
                {
                    await Application.Current.MainPage.Navigation.PushAsync(new ResetPassword());
                }
            };
        }
    }
}
