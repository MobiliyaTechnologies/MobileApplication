using CoreAnimation;
using CoreGraphics;
using CSU_PORTABLE.iOS.Utils;
using CSU_PORTABLE.Models;
using CSU_PORTABLE.Utils;
using Foundation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using UIKit;

namespace CSU_PORTABLE.iOS
{
    public partial class FeedbackViewController : BaseController
    {
        UILabel FeedbackHomeHeader, FeedbackHomeSubHeader;
        public static int classRoomId { get; set; }
        LoadingOverlay loadingOverlay;
        public static List<QuestionModel> questionList;
        UIPickerViewModel modelClassRooms;
        UIPickerView classRoomPicker;
        public static List<RoomModel> roomsList;
        public FeedbackViewController(IntPtr handle) : base(handle)
        {
        }

        public async override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            this.NavigationController.NavigationBarHidden = false;
            this.NavigationController.NavigationBar.TintColor = UIColor.White;
            this.NavigationController.NavigationBar.BarTintColor = IOSUtil.PrimaryColor;
            this.NavigationController.NavigationBar.BarStyle = UIBarStyle.BlackTranslucent;


            NavigationItem.SetRightBarButtonItem(
                new UIBarButtonItem(UIImage.FromBundle("a")
                    , UIBarButtonItemStyle.Plain
                    , (sender, args) =>
                    {
                        SidebarController.ToggleMenu();
                    }), true);

            // Added for showing loading screen

            CreateFeedbackDashboard();
            if (roomsList == null || questionList == null)
            {
                await GetRooms();
                await GetQuestionList();
            }
            else
            {
                BindClassRooms(roomsList);
            }
            if (questionList == null)
            {
                await GetQuestionList();
            }
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

        }

        #region " Events "

        private void BtnNext_TouchUpInside(object sender, EventArgs e)
        {
            if (classRoomId > 0 && questionList != null)
            {
                if (questionList.Count > 0)
                {
                    var QuestionsViewController = Storyboard.InstantiateViewController("QuestionsViewController") as QuestionsViewController;
                    QuestionsViewController.NavigationItem.SetHidesBackButton(true, false);
                    QuestionsViewController.questionList = questionList;
                    QuestionsViewController.selectedClassRoom = classRoomId;
                    QuestionsViewController.selectedClassRoomDesc = roomsList.Find(x => x.RoomId == classRoomId).RoomName;
                    NavController.PushViewController(QuestionsViewController, true);
                }
                else
                {
                    IOSUtil.ShowAlert("No Questions defined.");
                }
            }
            else
            {
                IOSUtil.ShowMessage("Select room.", loadingOverlay, this);
            }

        }

        #endregion

        #region " Custom Function "

        private async Task GetQuestionList()
        {
            var response = await InvokeApi.Invoke(Constants.API_GET_QUESTION_ANSWERS, string.Empty, HttpMethod.Get, PreferenceHandler.GetToken());
            if (response.StatusCode != 0)
            {
                getQuestionListResponse(response);
            }
        }

        private async void getQuestionListResponse(HttpResponseMessage restResponse)
        {
            if (restResponse != null && restResponse.StatusCode == HttpStatusCode.OK && restResponse.Content != null)
            {
                string strContent = await restResponse.Content.ReadAsStringAsync();
                JArray array = JArray.Parse(strContent);
                questionList = array.ToObject<List<QuestionModel>>();


            }
            else if (restResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                await IOSUtil.RefreshToken(this, loadingOverlay);
                //await GetQuestionList();
            }
            else
            {
                IOSUtil.ShowAlert("Error Occured");
            }
        }

        private void CreateFeedbackDashboard()
        {

            this.View.BackgroundColor = IOSUtil.PrimaryColor;
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

        public async Task GetRooms()
        {
            var bounds = UIScreen.MainScreen.Bounds;
            // show the loading overlay on the UI thread using the correct orientation sizing
            loadingOverlay = new LoadingOverlay(bounds);
            View.Add(loadingOverlay);
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
                roomsList = JsonConvert.DeserializeObject<List<RoomModel>>(strContent);
                if (roomsList.Count > 0)
                {
                    classRoomId = roomsList[0].RoomId;
                    BindClassRooms(roomsList);
                }
                else
                {
                    UILabel lblRemark = new UILabel()
                    {
                        Frame = new CGRect(0, this.NavigationController.NavigationBar.Bounds.Bottom + 250, View.Bounds.Width, 40),
                        Text = "No rooms found!",
                        Font = UIFont.FromName("Futura-Medium", 15f),
                        TextColor = UIColor.White,
                        BackgroundColor = UIColor.Clear,
                        LineBreakMode = UILineBreakMode.WordWrap,
                        Lines = 1,
                        TextAlignment = UITextAlignment.Center
                    };
                    View.AddSubviews(lblRemark);
                    loadingOverlay.Hide();
                }

            }
            else
            {
                IOSUtil.ShowMessage("No rooms found!", loadingOverlay, this);
            }
        }


        private void BindClassRooms(List<RoomModel> roomsList)
        {
            if (classRoomPicker == null)
            {
                modelClassRooms = new ClassRoomPickerViewModel(roomsList);

                classRoomPicker = new UIPickerView()
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
            }
            if (loadingOverlay != null)
            {
                loadingOverlay.Hide();
            }
        }

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
                    Tag = classRoooms[(int)row].RoomId
                };

                return view;
            }

            public override void Selected(UIPickerView pickerView, nint row, nint component)
            {
                classRoomId = roomsList[(int)row].RoomId;

            }


        }

        #endregion
    }
}