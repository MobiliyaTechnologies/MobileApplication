using CoreGraphics;
using CSU_PORTABLE.iOS.Utils;
using CSU_PORTABLE.Models;
using CSU_PORTABLE.Utils;
using Foundation;
using Newtonsoft.Json;
using RestSharp;
using SidebarNavigation;
using System;
using UIKit;

namespace CSU_PORTABLE.iOS
{
    public partial class ThankYouViewController : BaseController
    {

        UILabel ThankYouHeader, ThankYouSubHeader;

        public ThankYouViewController(IntPtr handle) : base(handle)
        {
        }



        #region " Events "

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            this.NavigationController.NavigationBar.TintColor = UIColor.White;
            this.NavigationController.NavigationBar.BarTintColor = UIColor.FromRGB(33, 77, 43);
            this.NavigationController.NavigationBar.BarStyle = UIBarStyle.BlackTranslucent;

            NavigationItem.SetRightBarButtonItem(
                new UIBarButtonItem(UIImage.FromBundle("a")
                    , UIBarButtonItemStyle.Plain
                    , (sender, args) =>
                    {
                        SidebarController.ToggleMenu();
                    }), true);

            GetThankYouView();

        }

        private void BtnDone_TouchUpInside(object sender, EventArgs e)
        {
            var FeedbackViewController = Storyboard.InstantiateViewController("FeedbackViewController") as FeedbackViewController;
            FeedbackViewController.NavigationItem.SetHidesBackButton(true, false);
            NavController.PushViewController(FeedbackViewController, true);
        }

        #endregion

        #region " Custom Functions "

        private void GetThankYouView()
        {
            this.View.BackgroundColor = UIColor.FromRGB(30, 77, 43);
            ThankYouHeader = new UILabel()
            {
                Font = UIFont.FromName("Futura-Medium", 18f),
                TextColor = UIColor.White,
                BackgroundColor = UIColor.Clear,
                Frame = new CGRect(50, 250, View.Bounds.Width - 120, 100),
                Text = "Thank You so much for your Valueable feedback.",
                TextAlignment = UITextAlignment.Center,
                Lines = 3,
                LineBreakMode = UILineBreakMode.WordWrap,
                AutosizesSubviews = true
            };


            UIImageView imgThankYou = new UIImageView()
            {
                Frame = new CGRect((View.Bounds.Width / 2) - 75, 120, 150, 150),
                Image = UIImage.FromBundle("Thank_You_Icon.png"),
                BackgroundColor = UIColor.Clear
            };

            UIButton btnDone = new UIButton(UIButtonType.Custom);
            btnDone.SetTitle("Done", UIControlState.Normal);
            btnDone.Font = UIFont.FromName("Futura-Medium", 15f);
            btnDone.SetTitleColor(UIColor.FromRGB(30, 77, 43), UIControlState.Normal);
            btnDone.SetTitleColor(UIColor.Green, UIControlState.Focused);
            btnDone.TouchUpInside += BtnDone_TouchUpInside;
            btnDone.Frame = new CGRect((View.Bounds.Width / 2) - 40, 450, 80, 40);
            btnDone.BackgroundColor = UIColor.White;


            View.AddSubviews(ThankYouHeader, btnDone, imgThankYou);
        }



        #endregion
    }
}