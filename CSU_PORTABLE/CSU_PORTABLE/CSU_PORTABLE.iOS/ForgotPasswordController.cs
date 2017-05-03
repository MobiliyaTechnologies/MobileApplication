using Foundation;
using System;
using UIKit;
using CSU_PORTABLE.Models;
using CSU_PORTABLE.Utils;
using Newtonsoft.Json;
using CSU_PORTABLE.iOS.Utils;
using System.Net.Http;

namespace CSU_PORTABLE.iOS
{
    public partial class ForgotPasswordController : UIViewController
    {
        LoadingOverlay loadingOverlay;
        public ForgotPasswordController(IntPtr handle) : base(handle)
        {

        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            this.NavigationController.NavigationBarHidden = false;
            this.NavigationController.NavigationBar.TintColor = UIColor.White;
            this.NavigationController.NavigationBar.BarTintColor = UIColor.FromRGB(33, 77, 43);
            this.NavigationController.NavigationBar.BarStyle = UIBarStyle.BlackTranslucent;
            Email.AutocorrectionType = UITextAutocorrectionType.No;
            Email.ShouldReturn = delegate
            {
                // Changed this slightly to move the text entry to the next field.
                Email.ResignFirstResponder();
                return true;
            };
        }

        partial void ForgotButton_TouchUpInside(UIButton sender)
        {
            // Added for showing loading screen
            var bounds = UIScreen.MainScreen.Bounds;
            // show the loading overlay on the UI thread using the correct orientation sizing
            loadingOverlay = new LoadingOverlay(bounds);
            View.Add(loadingOverlay);

            if (string.IsNullOrEmpty(Email.Text.Trim()))
            {
                loadingOverlay.Hide();
                IOSUtil.ShowMessage("Enter valid Email.", loadingOverlay, this);
            }
            else
            {
                SubmitEmail(new ForgotPasswordModel(Email.Text.Trim()));
            }
        }

        public async void SubmitEmail(ForgotPasswordModel objModel)
        {
            var response = await InvokeApi.Invoke(Constants.API_FORGOT_PASSWORD, JsonConvert.SerializeObject(objModel), HttpMethod.Post);
            if (response.StatusCode != 0)
            {
                InvokeOnMainThread(() =>
                {
                    ForgotPasswordResponse(response);
                    loadingOverlay.Hide();
                });
            }
        }

        private async void ForgotPasswordResponse(HttpResponseMessage restResponse)
        {
            if (restResponse != null && restResponse.StatusCode == System.Net.HttpStatusCode.OK && restResponse.Content != null)
            {
                string strContent = await restResponse.Content.ReadAsStringAsync();
                ForgotPasswordResponseModel response = JsonConvert.DeserializeObject<ForgotPasswordResponseModel>(strContent);
                if (response != null && response.Status_Code == Constants.STATUS_CODE_SUCCESS)
                {

                    PreferenceHandler preferenceHandler = new PreferenceHandler();
                    preferenceHandler.setLoggedIn(false);
                    IOSUtil.ShowMessage("Please check your Email.", loadingOverlay, this);
                }
                else
                {
                    IOSUtil.ShowMessage("Please try again.", loadingOverlay, this);
                }
            }
            else
            {
                IOSUtil.ShowMessage("Please try again.", loadingOverlay, this);
            }
        }
    }
}