using CSU_PORTABLE.iOS.Utils;
using CSU_PORTABLE.Models;
using Foundation;
using RestSharp;
using System;
using UIKit;

namespace CSU_PORTABLE.iOS
{
    public partial class ChangePasswordController : UIViewController
    {
        public ChangePasswordController(IntPtr handle) : base(handle)
        {
            
        }

        partial void Submit_TouchUpInside(UIButton sender)
        {

        }



        //#region "Custom Functions"

        //private void ChangePassword(ChangePasswordModel model)
        //{
        //    DisableButton(buttonSubmit);
        //    progressBar.Visibility = ViewStates.Visible;

        //    RestClient client = new RestClient(Constants.SERVER_BASE_URL);
        //    Log.Debug(TAG, "ChangePassword() " + model.ToString());

        //    var request = new RestRequest(Constants.API_CHANGE_PASSWORD, Method.POST);
        //    request.RequestFormat = DataFormat.Json;
        //    request.AddBody(model);

        //    //RestResponse restResponse = (RestResponse)client.Execute(request);
        //    //LoginResponse(restResponse);
        //    client.ExecuteAsync(request, response =>
        //    {
        //        Console.WriteLine(response);
        //        if (response.StatusCode != 0)
        //        {
        //            Log.Debug(TAG, "async Response : " + response.ToString());
        //            RunOnUiThread(() => {
        //                ChangePasswordResponse((RestResponse)response);
        //            });
        //        }
        //    });
        //}

        //private void ChangePasswordResponse(RestResponse restResponse)
        //{
        //    if (restResponse != null && restResponse.StatusCode == System.Net.HttpStatusCode.OK && restResponse.Content != null)
        //    {
        //        ChangePasswordResponseModel response = JsonConvert.DeserializeObject<ChangePasswordResponseModel>(restResponse.Content);

        //        if (response.Status_Code == Constants.STATUS_CODE_SUCCESS)
        //        {
        //            ShowToast("Password Changed Successfully. Please check your Email.");
        //        }
        //        else
        //        {
        //            ShowToast("Failed to change password, Plesase try again later.");
        //        }
        //    }
        //    else
        //    {
        //        Log.Debug(TAG, "Login Failed");
        //        progressBar.Visibility = ViewStates.Gone;
        //        EnableButton(buttonSubmit);
        //        tvMessage.Text = "Error in changing password. Please try again.";
        //        tvMessage.Visibility = ViewStates.Visible;
        //        ShowToast("Error in changing password. Please try again.");
        //    }
        //    Submit.Enabled = true;
        //}

        //private void ShowToast(string message)
        //{
        //    if (toast != null)
        //    {
        //        toast.Cancel();
        //    }
        //    toast = Toast.MakeText(this, message, ToastLength.Short);
        //    toast.Show();
        //}
        //#endregion
    }
}