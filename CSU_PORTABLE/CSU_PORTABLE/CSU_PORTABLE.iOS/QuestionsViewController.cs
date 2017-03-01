using CoreGraphics;
using CSU_PORTABLE.iOS.Utils;
using CSU_PORTABLE.Models;
using CSU_PORTABLE.Utils;
using Foundation;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using UIKit;

namespace CSU_PORTABLE.iOS
{
    public partial class QuestionsViewController : BaseController
    {
        UILabel QuestionHeader, QuestionSubHeader;
        public static int SelectedAnswer { get; set; }
        LoadingOverlay loadingOverlay;
        PreferenceHandler prefHandler;
        UserDetails userdetail;
        public int selectedClassRoom;

        public QuestionsViewController(IntPtr handle) : base(handle)
        {
        }

        //// provide access to the navigation controller to all inheriting controllers
        //protected NavController NavController
        //{
        //    get
        //    {
        //        return (UIApplication.SharedApplication.Delegate as AppDelegate).RootViewController.NavController;
        //    }
        //}

        //// provide access to the storyboard to all inheriting controllers
        //public override UIStoryboard Storyboard
        //{
        //    get
        //    {
        //        return (UIApplication.SharedApplication.Delegate as AppDelegate).RootViewController.Storyboard;
        //    }
        //}


        #region " Events "

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            NavigationItem.SetRightBarButtonItem(
                new UIBarButtonItem(UIImage.FromBundle("a")
                    , UIBarButtonItemStyle.Plain
                    , (sender, args) =>
                    {
                        SidebarController.ToggleMenu();
                    }), true);

            SelectedAnswer = -1;
            prefHandler = new Utils.PreferenceHandler();
            userdetail = prefHandler.GetUserDetails();
            GetQuestionView();
        }


        private void BtnSubmit_TouchUpInside(object sender, EventArgs e)
        {
            // Added for showing loading screen
            var bounds = UIScreen.MainScreen.Bounds;
            // show the loading overlay on the UI thread using the correct orientation sizing
            loadingOverlay = new LoadingOverlay(bounds);
            View.Add(loadingOverlay);
            if (SelectedAnswer >= 0)
            {

                submitFeedback(userdetail.User_Id);
                //var ThankYouViewController = Storyboard.InstantiateViewController("ThankYouViewController") as ThankYouViewController;
                //ThankYouViewController.NavigationItem.SetHidesBackButton(true, false);
                //NavigationController.PushViewController(ThankYouViewController, true);
            }
            else
            {
                ShowMessage("Please select an option");
            }
        }

        private void BtnBack_TouchUpInside(object sender, EventArgs e)
        {
            //NavigationController.PopToRootViewController(true);
            NavigationController.PopViewController(true);
        }


        #endregion


        #region " Custom Functions "

        private void GetQuestionView()
        {
            this.View.BackgroundColor = UIColor.FromRGB(30, 77, 43);
            QuestionHeader = new UILabel()
            {
                Font = UIFont.FromName("Helvetica-Bold", 20f),
                TextColor = UIColor.White,
                BackgroundColor = UIColor.Clear,
                Frame = new CGRect(30, 120, View.Bounds.Width - 60, 50),
                Text = "How's the temperature right now?",
                TextAlignment = UITextAlignment.Center,
                Lines = 3,
                LineBreakMode = UILineBreakMode.WordWrap,
                AutosizesSubviews = true
            };


            QuestionSubHeader = new UILabel()
            {
                Font = UIFont.FromName("Helvetica", 15f),
                TextColor = UIColor.White,
                BackgroundColor = UIColor.Clear,
                LineBreakMode = UILineBreakMode.WordWrap,
                Text = "Let us know how do you feel about the temperature where you are now.",
                Frame = new CGRect(30, 150, View.Bounds.Width - 60, 100),
                Lines = 3,
                TextAlignment = UITextAlignment.Center
            };


            UIButton btnBack = new UIButton(UIButtonType.Custom);
            //btnBack.SetTitle("Back", UIControlState.Normal);
            //btnBack.Font = UIFont.FromName("Futura-Medium", 15f);
            //btnBack.SetTitleColor(UIColor.FromRGB(30, 77, 43), UIControlState.Normal);
            //btnBack.SetTitleColor(UIColor.Green, UIControlState.Focused);
            btnBack.SetImage(UIImage.FromBundle("Back_BTN_White.png"), UIControlState.Normal);
            btnBack.Layer.CornerRadius = 20;
            btnBack.TouchUpInside += BtnBack_TouchUpInside;
            btnBack.Frame = new CGRect((View.Bounds.Width / 2) - 60, 480, 40, 40);
            btnBack.BackgroundColor = UIColor.FromRGB(33, 77, 43);

            UIButton btnSubmit = new UIButton(UIButtonType.Custom);
            //btnSubmit.SetTitle("Submit", UIControlState.Normal);
            //btnSubmit.Font = UIFont.FromName("Futura-Medium", 15f);
            //btnSubmit.SetTitleColor(UIColor.FromRGB(30, 77, 43), UIControlState.Normal);
            //btnSubmit.SetTitleColor(UIColor.Green, UIControlState.Focused);
            btnSubmit.SetImage(UIImage.FromBundle("Tick_BTN_White.png"), UIControlState.Normal);
            btnSubmit.Layer.CornerRadius = 20;
            btnSubmit.TouchUpInside += BtnSubmit_TouchUpInside;
            btnSubmit.Frame = new CGRect((View.Bounds.Width / 2) + 20, 480, 40, 40);
            btnSubmit.BackgroundColor = UIColor.White;


            List<string> answers = new List<string>() { "Very Cold", "Cold", "Normal", "Hot", "Very Hot" };
            UISegmentedControl segAnswers = new UISegmentedControl(new CGRect(10, 300, View.Bounds.Width - 20, 70));
            segAnswers.ControlStyle = UISegmentedControlStyle.Bar;
            UITextAttributes attAnswersdefault = new UITextAttributes()
            {
                Font = UIFont.FromName("Futura-Medium", 12f),
                TextColor = UIColor.LightTextColor,
                TextShadowColor = UIColor.LightTextColor
            };
            UITextAttributes attAnswersSelected = new UITextAttributes()
            {
                Font = UIFont.FromName("Futura-Medium", 12f),
                TextColor = UIColor.FromRGB(30, 77, 43),
                TextShadowColor = UIColor.LightTextColor
            };
            segAnswers.TintColor = UIColor.White;
            //UIOffset offAnswers = new UIOffset(5, 5);
            segAnswers.SetTitleTextAttributes(attAnswersdefault, UIControlState.Normal);
            segAnswers.SetTitleTextAttributes(attAnswersSelected, UIControlState.Selected);
            //segAnswers.SetContentPositionAdjustment(offAnswers, UISegmentedControlSegment.Center, UIBarMetrics.Default);
            segAnswers.Layer.BorderColor = UIColor.Clear.CGColor;
            segAnswers.AutosizesSubviews = false;
            segAnswers.InsertSegment(answers[0], 0, false);
            segAnswers.InsertSegment(answers[1], 1, false);
            segAnswers.InsertSegment(answers[2], 2, false);
            segAnswers.InsertSegment(answers[3], 3, false);
            segAnswers.InsertSegment(answers[4], 4, false);
            segAnswers.SetImage(UIImage.FromBundle("Very_Cold_Icon_Opacity.png"), 0);
            segAnswers.SetImage(UIImage.FromBundle("Cold_Icon_Opacity.png"), 1);
            segAnswers.SetImage(UIImage.FromBundle("Normal_Icon_Opacity.png"), 2);
            segAnswers.SetImage(UIImage.FromBundle("Hot_Icon_Opacity.png"), 3);
            segAnswers.SetImage(UIImage.FromBundle("Very_Hot_Icon_Opacity.png"), 4);

            UILabel lblSelectedAnswer = new UILabel()
            {
                Frame = new CGRect(10, 390, View.Bounds.Width - 20, 30),
                Font = UIFont.FromName("Futura-Medium", 18f),
                TextColor = UIColor.White,
                TextAlignment = UITextAlignment.Center
            };

            segAnswers.ValueChanged += (sender, e) =>
            {
                SelectedAnswer = (int)(sender as UISegmentedControl).SelectedSegment;
                segAnswers.SetImage(UIImage.FromBundle("Very_Cold_Icon_Opacity.png"), 0);
                segAnswers.SetImage(UIImage.FromBundle("Cold_Icon_Opacity.png"), 1);
                segAnswers.SetImage(UIImage.FromBundle("Normal_Icon_Opacity.png"), 2);
                segAnswers.SetImage(UIImage.FromBundle("Hot_Icon_Opacity.png"), 3);
                segAnswers.SetImage(UIImage.FromBundle("Very_Hot_Icon_Opacity.png"), 4);
                lblSelectedAnswer.Text = answers[SelectedAnswer];
                switch (SelectedAnswer)
                {
                    case 0:
                        segAnswers.SetImage(UIImage.FromBundle("Very_Cold_Icon.png"), SelectedAnswer);
                        View.BackgroundColor = UIColor.FromRGB(36, 116, 169);
                        btnBack.BackgroundColor = UIColor.FromRGB(36, 116, 169);
                        this.NavigationController.NavigationBar.BarTintColor = UIColor.FromRGB(36, 116, 169);
                        break;
                    case 1:
                        segAnswers.SetImage(UIImage.FromBundle("Cold_Icon.png"), SelectedAnswer);
                        btnBack.BackgroundColor = UIColor.FromRGB(16, 84, 86);
                        View.BackgroundColor = UIColor.FromRGB(16, 84, 86);
                        this.NavigationController.NavigationBar.BarTintColor = UIColor.FromRGB(16, 84, 86);
                        break;
                    case 2:
                        segAnswers.SetImage(UIImage.FromBundle("Normal_Icon.png"), SelectedAnswer);
                        View.BackgroundColor = UIColor.FromRGB(33, 77, 43);
                        btnBack.BackgroundColor = UIColor.FromRGB(33, 77, 43);
                        this.NavigationController.NavigationBar.BarTintColor = UIColor.FromRGB(33, 77, 43);
                        break;
                    case 3:
                        segAnswers.SetImage(UIImage.FromBundle("Hot_Icon.png"), SelectedAnswer);

                        View.BackgroundColor = UIColor.FromRGB(204, 84, 48);
                        btnBack.BackgroundColor = UIColor.FromRGB(204, 84, 48);
                        this.NavigationController.NavigationBar.BarTintColor = UIColor.FromRGB(204, 84, 48);
                        break;
                    case 4:
                        segAnswers.SetImage(UIImage.FromBundle("Very_Hot_Icon.png"), SelectedAnswer);
                        btnBack.BackgroundColor = UIColor.FromRGB(214, 69, 66);
                        View.BackgroundColor = UIColor.FromRGB(214, 69, 66);
                        this.NavigationController.NavigationBar.BarTintColor = UIColor.FromRGB(214, 69, 66);
                        break;
                    default:
                        this.NavigationController.NavigationBar.BarTintColor = UIColor.FromRGB(36, 116, 169);
                        break;
                }

            };

            segAnswers.SetBackgroundImage(imageWithColor(segAnswers.BackgroundColor), UIControlState.Normal, UIBarMetrics.Default);
            segAnswers.SetBackgroundImage(imageWithColor(segAnswers.TintColor), UIControlState.Selected, UIBarMetrics.Default);
            segAnswers.SetDividerImage(imageWithColor(UIColor.Clear), UIControlState.Normal, UIControlState.Normal, UIBarMetrics.Default);


            View.AddSubviews(QuestionHeader, QuestionSubHeader, btnBack, btnSubmit, segAnswers, lblSelectedAnswer);
        }



        private UIImage imageWithColor(UIColor imgColor)
        {

            var imgRenderer = new UIGraphicsImageRenderer(new CGSize(1, 1));
            var img = imgRenderer.CreateImage((UIGraphicsImageRendererContext ctxt) =>
            {
                var superRed = UIColor.FromDisplayP3(1.358f, -0.074f, -0.012f, 1.0f);
                superRed.SetFill();
                UIColor.Black.SetStroke();
                var path = new UIBezierPath();
                path.MoveTo(new CGPoint(10, 10));
                path.AddLineTo(new CGPoint(90, 10));
                path.AddLineTo(new CGPoint(45, 90));
                path.ClosePath();
                path.Stroke();
                path.Fill();
            });

            return img;
        }

        private void submitFeedback(int userId)
        {
            FeedbackModel feedbackModel = new FeedbackModel();
            feedbackModel.QuestionId = 1;
            feedbackModel.ClassId = selectedClassRoom + 1;
            feedbackModel.AnswerId = SelectedAnswer + 1;
            feedbackModel.FeedbackDesc = "";

            RestClient client = new RestClient(Constants.SERVER_BASE_URL);
            var request = new RestRequest(Constants.API_GIVE_FEEDBACK + "/" + userId, Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(feedbackModel);

            client.ExecuteAsync(request, response =>
            {
                Console.WriteLine(response);
                if (response.StatusCode != 0)
                {
                    InvokeOnMainThread(() =>
                    {
                        submitFeedbackResponse((RestResponse)response);
                    });

                }
            });
        }

        private void submitFeedbackResponse(RestResponse restResponse)
        {
            if (restResponse != null && restResponse.StatusCode == System.Net.HttpStatusCode.OK && restResponse.Content != null)
            {
                GeneralResponseModel response = JsonConvert.DeserializeObject<GeneralResponseModel>(restResponse.Content);

                if (response.Status_Code == Constants.STATUS_CODE_SUCCESS)
                {
                    var ThankYouViewController = Storyboard.InstantiateViewController("ThankYouViewController") as ThankYouViewController;
                    ThankYouViewController.NavigationItem.SetHidesBackButton(true, false);
                    NavigationController.PushViewController(ThankYouViewController, true);
                    loadingOverlay.Hide();
                }
                else
                {
                    ShowMessage("Failed to submit feedback, please try again!");
                }
            }
            else
            {
                ShowMessage("Failed to submit feedback, please try again!");
            }
        }

        private void ShowMessage(string v)
        {
            //BTProgressHUD.ShowToast("Hello from Toast");
            loadingOverlay.Hide();
            //MessageLabel.Text = " " + v;
            UIAlertController alertController = UIAlertController.Create("Message", v, UIAlertControllerStyle.Alert);

            alertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, (action) => Console.WriteLine("OK Clicked.")));

            PresentViewController(alertController, true, null);

        }

        #endregion
    }
}