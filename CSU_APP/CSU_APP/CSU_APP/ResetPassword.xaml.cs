using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace CSU_APP
{
    public partial class ResetPassword : ContentPage
    {
        public ResetPassword()
        {
            InitializeComponent();
            this.Title = "Reset Password";
            performReset.Clicked += async (sender, e) =>
            {
                AuthenticationManager manager = AuthenticationManager.Instance;
                IsBusy = true;
                string newP = newPassword.Text;
                string conformP = confirmPassword.Text;
                string oldPassword = currentPassword.Text;
                bool result = await manager.resetPassword(oldPassword, newP);
                if (result)
                {
                    await DisplayAlert("Success", "Password updated", "OK");
                }
                else
                {
                    await DisplayAlert("Faliure", "Error while password update", "OK");
                }
            };
        }
    }
}
