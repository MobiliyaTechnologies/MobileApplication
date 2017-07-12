using Foundation;
using System;
using UIKit;
using CSU_PORTABLE.Models;
using System.Collections.Generic;
using Newtonsoft.Json;
using CSU_PORTABLE.Utils;
using CSU_PORTABLE.iOS.Utils;
using System.Net.Http;

namespace CSU_PORTABLE.iOS
{
    public partial class ClassRoomController : BaseController
    {

        LoadingOverlay loadingOverlay;
        public ClassRoomController(IntPtr handle) : base(handle)
        {
        }


        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            var bounds = UIScreen.MainScreen.Bounds;
            // show the loading overlay on the UI thread using the correct orientation sizing
            loadingOverlay = new LoadingOverlay(bounds);
            View.Add(loadingOverlay);
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            GetClassRooms();
        }


        public async void GetClassRooms()
        {
            //PreferenceHandler prefHandler = new PreferenceHandler();
            UserDetails userDetail = PreferenceHandler.GetUserDetails();
            var response = await InvokeApi.Invoke(Constants.API_GET_CLASS_ROOMS + "/" + userDetail.UserId, string.Empty, HttpMethod.Get);
            if (response.StatusCode != 0)
            {
                InvokeOnMainThread(() =>
                {
                    CheckClassRoomsResponse(response);
                    loadingOverlay.Hide();
                });
            }
            //RestClient client = new RestClient(Constants.SERVER_BASE_URL);


            //var request = new RestRequest(Constants.API_GET_CLASS_ROOMS + "/" + userDetail.User_Id, Method.GET);
            //request.RequestFormat = DataFormat.Json;
            //client.ExecuteAsync(request, response =>
            //{
            //    Console.WriteLine(response);

            //});
        }

        private async void CheckClassRoomsResponse(HttpResponseMessage restResponse)
        {
            if (restResponse != null && restResponse.StatusCode == System.Net.HttpStatusCode.OK && restResponse.Content != null)
            {
                string strContent = await restResponse.Content.ReadAsStringAsync();
                List<ClassRoomModel> classRoomsList = JsonConvert.DeserializeObject<List<ClassRoomModel>>(strContent);
                BindClassRooms(classRoomsList);
            }
            else
            {
                IOSUtil.ShowMessage("No Class Rooms", loadingOverlay, this);
            }
        }


        private void BindClassRooms(List<ClassRoomModel> classRoomsList)
        {
            //List<ClassRoomModel> classRooms = new List<ClassRoomModel>() {
            //    new ClassRoomModel() { ClassRoomDesc= "ClassRoom1",ClassRoomId="C1",SensorId="S1"},
            //    new ClassRoomModel() { ClassRoomDesc= "ClassRoom2",ClassRoomId="C2",SensorId="S2"},
            //    new ClassRoomModel() { ClassRoomDesc= "ClassRoom3",ClassRoomId="C3",SensorId="S3"},
            //    new ClassRoomModel() { ClassRoomDesc= "ClassRoom4",ClassRoomId="C4",SensorId="S4"}
            //};

            UITableView _table;

            _table = new UITableView
            {
                Frame = new CoreGraphics.CGRect(0, 65, View.Bounds.Width, View.Bounds.Height - 65),
                Source = new ClassRoomSource(classRoomsList),
                RowHeight = 60
            };
            View.AddSubview(_table);
        }

        //private void ShowMessage(string v)
        //{
        //    loadingOverlay.Hide();
        //    UIAlertController alertController = UIAlertController.Create("Message", v, UIAlertControllerStyle.Alert);
        //    alertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, (action) => Console.WriteLine("OK Clicked.")));
        //    PresentViewController(alertController, true, null);

        //}
    }
}