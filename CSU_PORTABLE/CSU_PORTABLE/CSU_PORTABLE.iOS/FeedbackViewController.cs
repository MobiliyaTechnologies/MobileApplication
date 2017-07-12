using CoreAnimation;
using CoreGraphics;
using CSU_PORTABLE.iOS.Utils;
using CSU_PORTABLE.Models;
using CSU_PORTABLE.Utils;
using Foundation;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
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

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            this.NavigationController.NavigationBarHidden = false;
            this.NavigationController.NavigationBar.TintColor = UIColor.White;
            this.NavigationController.NavigationBar.BarTintColor = UIColor.FromRGB(0, 102, 153);
            this.NavigationController.NavigationBar.BarStyle = UIBarStyle.BlackTranslucent;
            classRoomId = 0;

            NavigationItem.SetRightBarButtonItem(
                new UIBarButtonItem(UIImage.FromBundle("a")
                    , UIBarButtonItemStyle.Plain
                    , (sender, args) =>
                    {
                        SidebarController.ToggleMenu();
                    }), true);

            // Added for showing loading screen
            var bounds = UIScreen.MainScreen.Bounds;
            // show the loading overlay on the UI thread using the correct orientation sizing
            loadingOverlay = new LoadingOverlay(bounds);
            View.Add(loadingOverlay);
            GetRooms();
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
                IOSUtil.ShowMessage("Select class room.", loadingOverlay, this);
            }
        }

        #endregion

        #region " Custom Function "

        private void CreateFeedbackDashboard()
        {

            this.View.BackgroundColor = UIColor.FromRGB(0, 102, 153);
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
            btnNext.SetImage(UIImage.FromBundle("Next_BTN_White.png"), UIControlState.Normal);
            btnNext.Layer.CornerRadius = 20;
            btnNext.TouchUpInside += BtnNext_TouchUpInside;
            btnNext.Frame = new CGRect((View.Bounds.Width / 2) - 20, 480, 40, 40);
            btnNext.BackgroundColor = UIColor.White;
            View.AddSubviews(FeedbackHomeHeader, FeedbackHomeSubHeader, btnNext);
        }

        public async void GetRooms()
        {
            //PreferenceHandler prefHandler = new PreferenceHandler();
            UserDetails userDetail = PreferenceHandler.GetUserDetails();
            var response = await InvokeApi.Invoke(Constants.API_GET_ALL_ROOMS, string.Empty, HttpMethod.Get, PreferenceHandler.GetToken());
            if (response.StatusCode != 0)
            {
                InvokeOnMainThread(() =>
                {
                    CheckClassRoomsResponse(response);
                    loadingOverlay.Hide();
                });
            }
        }

        private async void CheckClassRoomsResponse(HttpResponseMessage restResponse)
        {
            if (restResponse != null && restResponse.StatusCode == System.Net.HttpStatusCode.OK && restResponse.Content != null)
            {
                string strContent = await restResponse.Content.ReadAsStringAsync();
                List<RoomModel> roomsList = JsonConvert.DeserializeObject<List<RoomModel>>(strContent);
                BindClassRooms(roomsList);
            }
            else
            {
                IOSUtil.ShowMessage("No Class Rooms", loadingOverlay, this);
            }
        }


        private void BindClassRooms(List<RoomModel> roomsList)
        {
            UIPickerViewModel modelClassRooms = new ClassRoomPickerViewModel(roomsList);
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
            View.AddSubviews(classRoomPicker);
            classRoomPicker.Select(0, 0, true);
            loadingOverlay.Hide();
        }

        //private void ShowMessage(string v)
        //{
        //    //BTProgressHUD.ShowToast("Hello from Toast");
        //    if (loadingOverlay != null)
        //    {
        //        loadingOverlay.Hide();
        //    }
        //    //MessageLabel.Text = " " + v;
        //    UIAlertController alertController = UIAlertController.Create("Message", v, UIAlertControllerStyle.Alert);

        //    alertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, (action) => Console.WriteLine("OK Clicked.")));

        //    PresentViewController(alertController, true, null);

        //}
        #endregion




        #region " Picker "

        public class ClassRoomPickerViewModel : UIPickerViewModel
        {
            List<RoomModel> classRoooms;
            public int selectedClassRoom;
            public ClassRoomPickerViewModel()
            {

            }

            public ClassRoomPickerViewModel(List<RoomModel> component)
            {

                this.classRoooms = component;
            }

            public override nint GetRowsInComponent(UIPickerView pickerView, nint component)
            {

                return classRoooms.Count;
            }

            public override string GetTitle(UIPickerView pickerView, nint row, nint component)
            {
                return classRoooms[(int)row].RoomName;
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
                    Text = classRoooms[(int)row].RoomName,
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