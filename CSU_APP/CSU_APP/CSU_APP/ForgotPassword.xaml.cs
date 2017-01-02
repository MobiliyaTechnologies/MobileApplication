using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace CSU_APP
{
    public partial class ForgotPassword : ContentPage
    {
        public ForgotPassword()
        {
            AuthenticationManager manager = AuthenticationManager.Instance;
            InitializeComponent();
            this.Title = "Forgot Password";

            performForgotPassword.Clicked += async (sender, e) => {
                IsBusy = true;
                string userName = Uname.Text;

                Models.ForgotPasswordModel responseObj = await manager.forgotPassword(userName);
                if (responseObj != null)
                {
                    Msg.Text = responseObj.Message + " Please check your Email.";
                } else
                {
                    Msg.Text = "Error Reseting Password, Please try again !";
                    IsBusy = false;
                }
                
            };
        }
    }
}
