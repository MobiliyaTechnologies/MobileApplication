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
            this.Title = "Change Password";
            performReset.Clicked += async (sender, e) =>
            {
                AuthenticationManager manager = AuthenticationManager.Instance;
                IsBusy = true;
                string newP = newPassword.Text;
                string conformP = confirmPassword.Text;
                string oldPassword = currentPassword.Text;

                if(oldPassword == null || oldPassword.Length < 1)
                {
                    await DisplayAlert("Reset Password", "Enter current password", "OK");
                }
                else if (newP == null || newP.Length < 1)
                {
                    await DisplayAlert("Reset Password", "Enter new password", "OK");
                }
                else if (conformP == null || conformP.Length < 1 || !newP.Equals(conformP))
                {
                    await DisplayAlert("Reset Password", "Password do not match", "OK");
                } else
                {
                    var preferenceHandler = DependencyService.Get<IPreferencesHandler>();
                    string email = preferenceHandler.GetUserDetails().Email;
                    if (email != null && email != "")
                    {
                        bool result = await manager.changePassword(email, oldPassword, newP);
                        if (result)
                        {
                            await DisplayAlert("Success", "Password updated", "OK");
                        }
                        else
                        {
                            await DisplayAlert("Faliure", "Error while password update, Please try again.", "OK");
                        }
                    }
                }
                
            };
        }
    }
}
