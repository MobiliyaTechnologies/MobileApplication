using CoreAnimation;
using CoreGraphics;
using CSU_PORTABLE.iOS.Utils;
using CSU_PORTABLE.Models;
using CSU_PORTABLE.Utils;
using Foundation;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using UIKit;

namespace CSU_PORTABLE.iOS
{
    public partial class FeedbackViewController : BaseController
    {
        UILabel FeedbackHomeHeader, FeedbackHomeSubHeader;
        public static int classRoomId { get; set; }
        LoadingOverlay loadingOverlay;

        public FeedbackViewController(IntPtr handle) : base(handle)
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

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            classRoomId = -1;
            // Added for showing loading screen
            var bounds = UIScreen.MainScreen.Bounds;
            // show the loading overlay on the UI thread using the correct orientation sizing
            loadingOverlay = new LoadingOverlay(bounds);
            View.Add(loadingOverlay);
            GetClassRooms();
            CreateFeedbackDashboard();

        }

        #region " Events "

        private void BtnNext_TouchUpInside(object sender, EventArgs e)
        {
            if (classRoomId >= 0)
            {
                var QuestionsViewController = Storyboard.InstantiateViewController("QuestionsViewController") as QuestionsViewController;
                QuestionsViewController.NavigationItem.SetHidesBackButton(true, false);
                QuestionsViewController.selectedClassRoom = classRoomId;
                NavController.PushViewController(QuestionsViewController, true);
            }
            else
            {
                ShowMessage("Select class room.");
            }
        }

        #endregion

        #region " Custom Function "

        private void CreateFeedbackDashboard()
        {

            this.View.BackgroundColor = UIColor.FromRGB(30, 77, 43);
            FeedbackHomeHeader = new UILabel()
            {
                Font = UIFont.FromName("Helvetica-Bold", 20f),
                TextColor = UIColor.White,
                BackgroundColor = UIColor.Clear,
                Frame = new CGRect(30, 100, View.Bounds.Width - 60, 50),
                Text = "Where are you right now?",
                TextAlignment = UITextAlignment.Center,
                Lines = 2,
                LineBreakMode = UILineBreakMode.TailTruncation,
                AutosizesSubviews = true
            };


            FeedbackHomeSubHeader = new UILabel()
            {
                Font = UIFont.FromName("Futura-Medium", 15f),
                TextColor = UIColor.White,
                BackgroundColor = UIColor.Clear,
                LineBreakMode = UILineBreakMode.WordWrap,
                Text = "Let us know your exact location within the university campus to locate.",
                Frame = new CGRect(30, 120, View.Bounds.Width - 60, 100),
                Lines = 3,
                TextAlignment = UITextAlignment.Center
            };

            UIButton btnNext = new UIButton(UIButtonType.Custom);
            btnNext.SetTitle("Next", UIControlState.Normal);
            btnNext.Font = UIFont.FromName("Futura-Medium", 15f);
            btnNext.SetTitleColor(UIColor.FromRGB(30, 77, 43), UIControlState.Normal);
            btnNext.SetTitleColor(UIColor.Green, UIControlState.Focused);
            btnNext.TouchUpInside += BtnNext_TouchUpInside;
            btnNext.Frame = new CGRect((View.Bounds.Width / 2) - 40, 450, 80, 40);
            btnNext.BackgroundColor = UIColor.White;
            View.AddSubviews(FeedbackHomeHeader, FeedbackHomeSubHeader, btnNext);
        }

        public void GetClassRooms()
        {
            RestClient client = new RestClient(Constants.SERVER_BASE_URL);
            PreferenceHandler prefHandler = new PreferenceHandler();
            UserDetails userDetail = prefHandler.GetUserDetails();
            var request = new RestRequest(Constants.API_GET_CLASS_ROOMS + "/" + userDetail.User_Id, Method.GET);
            request.RequestFormat = DataFormat.Json;
            client.ExecuteAsync(request, response =>
            {
                Console.WriteLine(response);
                if (response.StatusCode != 0)
                {
                    InvokeOnMainThread(() =>
                    {
                        CheckClassRoomsResponse(response);

                    });
                }
            });
        }

        private void CheckClassRoomsResponse(IRestResponse restResponse)
        {
            List<ClassRoomModel> classRoomsList = new List<ClassRoomModel>();
            if (restResponse != null && restResponse.StatusCode == System.Net.HttpStatusCode.OK && restResponse.Content != null)
            {
                classRoomsList = JsonConvert.DeserializeObject<List<ClassRoomModel>>(restResponse.Content);
            }
            BindClassRooms(classRoomsList);
        }

        private void BindClassRooms(List<ClassRoomModel> classRoomsList)
        {
            UIPickerViewModel modelClassRooms = new ClassRoomPickerViewModel(classRoomsList);
            UIPickerView classRoomPicker = new UIPickerView()
            {
                Frame = new CGRect(50, 220, View.Bounds.Width - 100, 200),
                ShowSelectionIndicator = true,
                Model = modelClassRooms,
            };
            classRoomPicker.AccessibilityNavigationStyle = UIAccessibilityNavigationStyle.Automatic;
            classRoomPicker.ContentMode = UIViewContentMode.Center;
            var subViewTop = new UIView(new CGRect(classRoomPicker.Bounds.X - 20, classRoomPicker.Bounds.Y + 85, classRoomPicker.Bounds.Width, 1));
            var subViewBottom = new UIView(new CGRect(classRoomPicker.Bounds.X - 20, classRoomPicker.Bounds.Y + 115, classRoomPicker.Bounds.Width, 1));
            subViewTop.BackgroundColor = UIColor.White;
            subViewBottom.BackgroundColor = UIColor.White;
            classRoomPicker.AddSubview(subViewTop);
            classRoomPicker.AddSubview(subViewBottom);
            classRoomPicker.SelectedRowInComponent(0);
            View.AddSubviews(classRoomPicker);
            loadingOverlay.Hide();
        }

        private void ShowMessage(string v)
        {
            //BTProgressHUD.ShowToast("Hello from Toast");
            if (loadingOverlay != null)
            {
                loadingOverlay.Hide();
            }
            //MessageLabel.Text = " " + v;
            UIAlertController alertController = UIAlertController.Create("Message", v, UIAlertControllerStyle.Alert);

            alertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, (action) => Console.WriteLine("OK Clicked.")));

            PresentViewController(alertController, true, null);

        }
        #endregion


        #region " Picker "

        public class ClassRoomPickerViewModel : UIPickerViewModel
        {
            List<ClassRoomModel> classRoooms;
            public int selectedClassRoom;
            public ClassRoomPickerViewModel()
            {

            }

            public ClassRoomPickerViewModel(List<ClassRoomModel> component)
            {
                this.classRoooms = component;
            }

            public override nint GetRowsInComponent(UIPickerView pickerView, nint component)
            {
                return classRoooms.Count;
            }

            public override string GetTitle(UIPickerView pickerView, nint row, nint component)
            {
                return classRoooms[(int)row].ClassDescription;
            }

            public override nint GetComponentCount(UIPickerView pickerView)
            {

                return 1;
            }

            public override UIView GetView(UIPickerView pickerView, nint row, nint component, UIView view)
            {
                view = (UIView)new UILabel()
                {
                    Font = UIFont.FromName("Futura-CondensedMedium", 25f),
                    TextColor = UIColor.White,
                    BackgroundColor = UIColor.Clear,
                    Text = classRoooms[(int)row].ClassDescription,
                    TextAlignment = UITextAlignment.Center,
                };

                return view;
            }

            public override void Selected(UIPickerView pickerView, nint row, nint component)
            {
                classRoomId = (int)pickerView.SelectedRowInComponent(component);

            }


        }

        #endregion
    }
}