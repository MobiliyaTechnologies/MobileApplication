using CSU_PORTABLE.iOS.Utils;
using CSU_PORTABLE.Models;
using CSU_PORTABLE.Utils;
using Foundation;
using Newtonsoft.Json;
using System;
using UIKit;
using System.Net.Http;

namespace CSU_PORTABLE.iOS
{
    public partial class ChangePasswordController : UIViewController
    {
        PreferenceHandler prefHandler = null;
        UserDetails User = null;
        LoadingOverlay loadingOverlay;
        public ChangePasswordController(IntPtr handle) : base(handle)
        {
            this.prefHandler = new PreferenceHandler();
            User = prefHandler.GetUserDetails();

        }


        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            Username.Text = User.Email;

            Password.AutocorrectionType = UITextAutocorrectionType.No;
            ConfirmPassword.AutocorrectionType = UITextAutocorrectionType.No;
            Password.ShouldReturn = delegate
            {
                // Changed this slightly to move the text entry to the next field.
                ConfirmPassword.BecomeFirstResponder();
                return true;
            };
            ConfirmPassword.ShouldReturn = delegate
            {
                // Changed this slightly to move the text entry to the next field.
                ConfirmPassword.ResignFirstResponder();
                return true;
            };
        }

        partial void Submit_TouchUpInside(UIButton sender)
        {
            // Added for showing loading screen
            var bounds = UIScreen.MainScreen.Bounds;
            // show the loading overlay on the UI thread using the correct orientation sizing
            loadingOverlay = new LoadingOverlay(bounds);
            View.Add(loadingOverlay);

            Submit.Enabled = false;

            if (string.IsNullOrEmpty(User.Email))
            {
                IOSUtil.ShowMessage("Enter valid Email.", loadingOverlay, this);
            }
            else
            {
                string password = Password.Text.Trim();
                string confirmPassword = ConfirmPassword.Text.Trim();
                if (password != null && password.Length > 2)
                {
                    if (confirmPassword != null && confirmPassword.Length > 2)
                    {
                        ChangePasswordModel model = new ChangePasswordModel();
                        model.Email = User.Email;
                        model.Password = password;
                        model.New_Password = confirmPassword;
                        ChangePassword(model);
                    }
                    else
                    {
                        IOSUtil.ShowMessage("Enter valid confirm password", loadingOverlay, this);
                    }
                }
                else
                {
                    IOSUtil.ShowMessage("Enter valid password", loadingOverlay, this);
                }
            }
            Submit.Enabled = true;
        }



        #region "Custom Functions"

        private async void ChangePassword(ChangePasswordModel model)
        {
            var response = await InvokeApi.Invoke(Constants.API_CHANGE_PASSWORD, JsonConvert.SerializeObject(model), HttpMethod.Post);
            if (response.StatusCode != 0)
            {
                InvokeOnMainThread(() =>
                {
                    ChangePasswordResponse(response);
                });
            }
        }

        private async void ChangePasswordResponse(HttpResponseMessage restResponse)
        {
            if (restResponse != null && restResponse.StatusCode == System.Net.HttpStatusCode.OK && restResponse.Content != null)
            {
                string strContent = await restResponse.Content.ReadAsStringAsync();
                ChangePasswordResponseModel response = JsonConvert.DeserializeObject<ChangePasswordResponseModel>(strContent);

                if (response.Status_Code == Constants.STATUS_CODE_SUCCESS)
                {
                    IOSUtil.ShowMessage("Password Changed Successfully. Please check your Email.", loadingOverlay, this);
                }
                else
                {
                    IOSUtil.ShowMessage("Failed to change password, Plesase try again later.", loadingOverlay, this);
                }
            }
            else
            {
                IOSUtil.ShowMessage("Error in changing password. Please try again.", loadingOverlay, this);
            }
        }
        #endregion
    }
}