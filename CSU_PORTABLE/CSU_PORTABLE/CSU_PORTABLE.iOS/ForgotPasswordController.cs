using Foundation;
using System;
using UIKit;
using CSU_PORTABLE.Models;
using RestSharp;
using CSU_PORTABLE.Utils;
using Newtonsoft.Json;
using CSU_PORTABLE.iOS.Utils;

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
                ShowMessage("Enter valid Email.");
            }
            else
            {
                SubmitEmail(new ForgotPasswordModel(Email.Text.Trim()));
            }
        }

        // shows pop up messages
        private void ShowMessage(string v)
        {

            UIAlertController alertController = UIAlertController.Create("Message", v, UIAlertControllerStyle.Alert);
            alertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, (action) => Console.WriteLine("OK Clicked.")));
            PresentViewController(alertController, true, null);

        }


        public void SubmitEmail(ForgotPasswordModel objModel)
        {

            RestClient client = new RestClient(Constants.SERVER_BASE_URL);
            var request = new RestRequest(Constants.API_FORGOT_PASSWORD, Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(objModel);

            //RestResponse restResponse = (RestResponse)client.Execute(request);
            //ForgotPasswordResponse(restResponse);
            client.ExecuteAsync(request, response =>
            {
                Console.WriteLine(response);
                if (response.StatusCode != 0)
                {

                    InvokeOnMainThread(() =>
                    {
                        ForgotPasswordResponse((RestResponse)response);
                        loadingOverlay.Hide();
                    });
                }
            });

        }

        private void ForgotPasswordResponse(RestResponse restResponse)
        {
            if (restResponse != null && restResponse.StatusCode == System.Net.HttpStatusCode.OK && restResponse.Content != null)
            {

                ForgotPasswordResponseModel response = JsonConvert.DeserializeObject<ForgotPasswordResponseModel>(restResponse.Content);

                if (response != null && response.Status_Code == Constants.STATUS_CODE_SUCCESS)
                {

                    PreferenceHandler preferenceHandler = new PreferenceHandler();
                    preferenceHandler.setLoggedIn(false);
                    ShowMessage("Please check your Email.");
                }
                else
                {
                    ShowMessage("Please try again.");
                }
            }
            else
            {
                ShowMessage("Please try again.");

            }
        }
    }
}