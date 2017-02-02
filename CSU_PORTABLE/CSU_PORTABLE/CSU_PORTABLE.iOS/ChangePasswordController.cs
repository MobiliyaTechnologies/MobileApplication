using CSU_PORTABLE.iOS.Utils;
using CSU_PORTABLE.Models;
using CSU_PORTABLE.Utils;
using Foundation;
using Newtonsoft.Json;
using RestSharp;
using System;
using UIKit;

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
                ShowMessage("Enter valid Email.");
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
                        ShowMessage("Enter valid confirm password");
                    }
                }
                else
                {
                    ShowMessage("Enter valid password");
                }
            }
            Submit.Enabled = true;
        }



        #region "Custom Functions"

        private void ChangePassword(ChangePasswordModel model)
        {
            RestClient client = new RestClient(Constants.SERVER_BASE_URL);
            var request = new RestRequest(Constants.API_CHANGE_PASSWORD, Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(model);
            client.ExecuteAsync(request, response =>
            {
                Console.WriteLine(response);
                if (response.StatusCode != 0)
                {
                    InvokeOnMainThread(() =>
                    {
                        ChangePasswordResponse((RestResponse)response);
                    });
                }
            });
        }

        private void ChangePasswordResponse(RestResponse restResponse)
        {
            if (restResponse != null && restResponse.StatusCode == System.Net.HttpStatusCode.OK && restResponse.Content != null)
            {
                ChangePasswordResponseModel response = JsonConvert.DeserializeObject<ChangePasswordResponseModel>(restResponse.Content);

                if (response.Status_Code == Constants.STATUS_CODE_SUCCESS)
                {
                    ShowMessage("Password Changed Successfully. Please check your Email.");
                }
                else
                {
                    ShowMessage("Failed to change password, Plesase try again later.");
                }
            }
            else
            {
                ShowMessage("Error in changing password. Please try again.");
            }

        }

        private void ShowMessage(string v)
        {
            if (loadingOverlay != null)
            {
                loadingOverlay.Hide();
            }
            UIAlertController alertController = UIAlertController.Create("Message", v, UIAlertControllerStyle.Alert);
            alertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, (action) => Console.WriteLine("OK Clicked.")));
            PresentViewController(alertController, true, null);

        }
        #endregion
    }
}