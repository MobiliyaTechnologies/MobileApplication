using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using CSU_PORTABLE.Droid.Utils;
using CSU_PORTABLE.Utils;
using Newtonsoft.Json;
using System.Net.Http;
using CSU_PORTABLE.Models;
using Android.Content.PM;

namespace CSU_PORTABLE.Droid.UI
{
    [Activity(Label = "Configuration", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class ConfigActivity : Activity
    {
        private EditText textConfigURL;
        public Button SubmitButton { get; private set; }
        PreferenceHandler preferenceHandler;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Config);
            textConfigURL = FindViewById<EditText>(Resource.Id.textConfigURL);
            SubmitButton = FindViewById<Button>(Resource.Id.SubmitButton);

            preferenceHandler = new PreferenceHandler();
            SubmitButton.Click += SubmitButton_Click;
            // Create your application here
        }

        private async void SubmitButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textConfigURL.Text))
            {
                Utils.Utils.ShowToast(this, "Enter Server Url.");
            }
            else
            {
                string domain = textConfigURL.Text;
                preferenceHandler.SetDomainKey(domain);
                InvokeApi.SetDomainUrl(domain);
                ProgressDialog dialog = new ProgressDialog(this);
                dialog.SetTitle("Loading...");
                dialog.Show();
                var response = await InvokeApi.Invoke(Constants.API_GET_MOBILE_CONFIGURATION, string.Empty, HttpMethod.Get);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string strContent = await response.Content.ReadAsStringAsync();
                    var config = JsonConvert.DeserializeObject<B2CConfiguration>(strContent);
                    dialog.Dismiss();
                    if (string.IsNullOrEmpty(config.B2cAuthorizeURL) || string.IsNullOrEmpty(config.B2cChangePasswordPolicy) || string.IsNullOrEmpty(config.B2cChangePasswordURL) || string.IsNullOrEmpty(config.B2cClientId)
                        || string.IsNullOrEmpty(config.B2cClientSecret) || string.IsNullOrEmpty(config.B2cRedirectUrl) || string.IsNullOrEmpty(config.B2cSignInPolicy) || string.IsNullOrEmpty(config.B2cSignUpPolicy)
                        || string.IsNullOrEmpty(config.B2cTenant) || string.IsNullOrEmpty(config.B2cTokenURL) || string.IsNullOrEmpty(config.B2cTokenURLIOS))
                    {
                        Utils.Utils.ShowToast(this, "Invalid Configuration details");
                    }
                    else
                    {
                        B2CConfigManager.GetInstance().Initialize(config);
                        preferenceHandler.SetConfig(strContent);
                        Intent intent = new Intent(Application.Context, typeof(LoginActivity));
                        StartActivity(intent);
                        Finish();
                    }

                }
                else
                {
                    Utils.Utils.ShowToast(this, response.ReasonPhrase);
                    dialog.Dismiss();
                }
            }
        }
    }
}