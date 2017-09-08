using CoreGraphics;
using EM_PORTABLE.iOS.Utils;
using EM_PORTABLE.Models;
using EM_PORTABLE.Utils;
using Foundation;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using UIKit;

namespace EM_PORTABLE.iOS
{
    public partial class ConfigurationController : BaseController
    {
        private UIButton ButtonSubmit;
        private UITextField TextFieldConfig;
        //PreferenceHandler preferenceHandler;
        LoadingOverlay loadingOverlay;
        private UIButton ButtonCancel;

        public ConfigurationController(IntPtr handle) : base(handle)
        {

        }



        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            this.View.BackgroundColor = IOSUtil.PrimaryColor;
            this.NavigationController.NavigationBarHidden = true;
            this.NavigationItem.SetHidesBackButton(false, false);
            this.Title = "EM Configuration";
            //preferenceHandler = new PreferenceHandler();
            UIView paddingView = new UIView(new CGRect(5, 5, 5, 20));
            TextFieldConfig = new UITextField()
            {
                Font = UIFont.FromName("Helvetica", 15f),
                TextColor = IOSUtil.PrimaryColor,
                BackgroundColor = UIColor.White,
                Frame = new CGRect((View.Bounds.Width / 2) - 120, 250, 240, 30),
                Placeholder = "Enter Domain URL",
                TextAlignment = UITextAlignment.Left,
                AutocorrectionType = UITextAutocorrectionType.No,
                LeftView = paddingView,
                LeftViewMode = UITextFieldViewMode.Always,
                BorderStyle = UITextBorderStyle.RoundedRect,
                TintColor = IOSUtil.SecondaryColor
            };

            ButtonSubmit = new UIButton(UIButtonType.Custom);
            ButtonSubmit.SetTitle("Submit", UIControlState.Normal);
            ButtonSubmit.Font = UIFont.FromName("Futura-Medium", 15f);
            ButtonSubmit.SetTitleColor(UIColor.White, UIControlState.Normal);
            ButtonSubmit.SetTitleColor(UIColor.White, UIControlState.Focused);
            ButtonSubmit.Frame = new CGRect((View.Bounds.Width / 2) - 120, 300, 240, 30);
            ButtonSubmit.BackgroundColor = IOSUtil.SecondaryColor;
            ButtonSubmit.TouchUpInside += ButtonSubmit_TouchUpInside;
            View.AddSubviews(TextFieldConfig, ButtonSubmit);
        }

        private void ButtonCancel_TouchUpInside(object sender, EventArgs e)
        {
            NavController.PopToRootViewController(true);
        }

        private async void ButtonSubmit_TouchUpInside(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(TextFieldConfig.Text))
            {
                IOSUtil.ShowMessage("Enter Domain Url.", null, this);
            }
            else
            {
                string domain = TextFieldConfig.Text;
                PreferenceHandler.SetDomainKey(domain);
                InvokeApi.SetDomainUrl(domain);
                var response = await InvokeApi.Invoke(Constants.API_GET_MOBILE_CONFIGURATION, string.Empty, HttpMethod.Get);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string strContent = await response.Content.ReadAsStringAsync();
                    var config = JsonConvert.DeserializeObject<B2CConfiguration>(strContent);
                    if (string.IsNullOrEmpty(config.B2cAuthorizeURL) || string.IsNullOrEmpty(config.B2cChangePasswordPolicy) || string.IsNullOrEmpty(config.B2cChangePasswordURL) || string.IsNullOrEmpty(config.B2cClientId)
                        || string.IsNullOrEmpty(config.B2cClientSecret) || string.IsNullOrEmpty(config.B2cRedirectUrl) || string.IsNullOrEmpty(config.B2cSignInPolicy) || string.IsNullOrEmpty(config.B2cSignUpPolicy)
                        || string.IsNullOrEmpty(config.B2cTenant) || string.IsNullOrEmpty(config.B2cTokenURL) || string.IsNullOrEmpty(config.B2cTokenURLIOS))
                    {
                        IOSUtil.ShowMessage("Invalid Configuration details", null, this);
                        PreferenceHandler.SetDomainKey(string.Empty);
                    }
                    else
                    {
                        B2CConfigManager.GetInstance().Initialize(config);
                        PreferenceHandler.SetConfig(strContent);
                        var ViewController = (ViewController)Storyboard.InstantiateViewController("ViewController");
                        NavController.PushViewController(ViewController, false);
                    }

                }
                else
                {
                    IOSUtil.ShowMessage(response.ReasonPhrase, null, this);
                    PreferenceHandler.SetDomainKey(string.Empty);
                }
            }
        }
    }
}