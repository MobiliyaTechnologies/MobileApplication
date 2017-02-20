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

            SelectedAnswer = 0;
            prefHandler = new Utils.PreferenceHandler();
            userdetail = prefHandler.GetUserDetails();
            GetQuestionView();
            //UITableView _table;

            //_table = new UITableView
            //{
            //    Frame = new CoreGraphics.CGRect(10, 250, View.Bounds.Width - 20, 70),
            //    RowHeight = 50,
            //    Source = new AnswerSource(answers),
            //};
            //View.AddSubview(_table);
            // GetAnswers(answers);

        }

        private void BtnSubmit_TouchUpInside(object sender, EventArgs e)
        {
            // Added for showing loading screen
            var bounds = UIScreen.MainScreen.Bounds;
            // show the loading overlay on the UI thread using the correct orientation sizing
            loadingOverlay = new LoadingOverlay(bounds);
            View.Add(loadingOverlay);
            if (SelectedAnswer != 0)
            {

                submitFeedback(userdetail.User_Id);
                var ThankYouViewController = Storyboard.InstantiateViewController("ThankYouViewController") as ThankYouViewController;
                ThankYouViewController.NavigationItem.SetHidesBackButton(true, false);
                NavigationController.PushViewController(ThankYouViewController, true);
            }
            else
            {
                ShowMessage("Please select an option");
            }
        }

        private void BtnBack_TouchUpInside(object sender, EventArgs e)
        {
            NavigationController.PopToRootViewController(true);
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
            btnBack.SetTitle("Back", UIControlState.Normal);
            btnBack.Font = UIFont.FromName("Futura-Medium", 15f);
            btnBack.SetTitleColor(UIColor.FromRGB(30, 77, 43), UIControlState.Normal);
            btnBack.SetTitleColor(UIColor.Green, UIControlState.Focused);
            btnBack.TouchUpInside += BtnBack_TouchUpInside;
            btnBack.Frame = new CGRect((View.Bounds.Width / 2) - 80, 450, 80, 40);
            btnBack.BackgroundColor = UIColor.White;

            UIButton btnSubmit = new UIButton(UIButtonType.Custom);
            btnSubmit.SetTitle("Submit", UIControlState.Normal);
            btnSubmit.Font = UIFont.FromName("Futura-Medium", 15f);
            btnSubmit.SetTitleColor(UIColor.FromRGB(30, 77, 43), UIControlState.Normal);
            btnSubmit.SetTitleColor(UIColor.Green, UIControlState.Focused);
            btnSubmit.TouchUpInside += BtnSubmit_TouchUpInside;
            btnSubmit.Frame = new CGRect((View.Bounds.Width / 2) + 20, 450, 80, 40);
            btnSubmit.BackgroundColor = UIColor.White;


            List<string> answers = new List<string>() { "very cold", "cold", "normal", "hot", "very hot" };
            UISegmentedControl segAnswers = new UISegmentedControl(new CGRect(10, 250, View.Bounds.Width - 20, 50));
            segAnswers.ControlStyle = UISegmentedControlStyle.Plain;

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
            segAnswers.TintColor = UIColor.FromWhiteAlpha(1f, 0.7f);
            //UIOffset offAnswers = new UIOffset(5, 5);
            segAnswers.SetTitleTextAttributes(attAnswersdefault, UIControlState.Normal);
            segAnswers.SetTitleTextAttributes(attAnswersSelected, UIControlState.Selected);
            //segAnswers.SetContentPositionAdjustment(offAnswers, UISegmentedControlSegment.Center, UIBarMetrics.Default);
            segAnswers.Layer.BorderColor = UIColor.Clear.CGColor;
            segAnswers.AutosizesSubviews = true;
            segAnswers.InsertSegment(answers[0], 1, false);
            segAnswers.InsertSegment(answers[1], 2, false);
            segAnswers.InsertSegment(answers[2], 3, false);
            segAnswers.InsertSegment(answers[3], 4, false);
            segAnswers.InsertSegment(answers[4], 5, false);
            segAnswers.ValueChanged += (sender, e) =>
            {
                SelectedAnswer = (int)(sender as UISegmentedControl).SelectedSegment;
            };


            View.AddSubviews(QuestionHeader, QuestionSubHeader, btnBack, btnSubmit, segAnswers);
        }

        private void submitFeedback(int userId)
        {
            FeedbackModel feedbackModel = new FeedbackModel();
            feedbackModel.QuestionId = 1;
            feedbackModel.ClassId = selectedClassRoom;
            feedbackModel.AnswerId = SelectedAnswer;
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
                    submitFeedbackResponse((RestResponse)response);
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