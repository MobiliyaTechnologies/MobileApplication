using Android.App;
using Android.Widget;
using Android.OS;
using Android.Gms.Common;
using Firebase.Messaging;
using Firebase.Iid;
using Android.Util;
using System;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using CSU_PORTABLE.Droid.Utils;
using CSU_PORTABLE.Models;
using RestSharp;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Android.Content;
using Android.Support.V7.App;
using Android.Support.V4.Widget;
using Android.Support.Design.Widget;
using Android.Views;
using Newtonsoft.Json;
using CSU_PORTABLE.Utils;

namespace CSU_PORTABLE.Droid.UI
{
    [Activity(Label = "CSU APP", MainLauncher = false, Icon = "@drawable/icon", Theme = "@style/MyTheme")]
    public class MainActivity : AppCompatActivity, IOnMapReadyCallback
    {

        //TextView msgText;
        const string TAG = "MainActivity";
        public static string KEY_USER_ROLE = "user_role";
        Toast toast;
        GoogleMap map;
        MapFragment _myMapFragment;
        List<MeterDetails> meterList = null;
        List<MonthlyConsumptionDetails> monthlyConsumptionList = null;
        bool IsMapReady = false;
        DrawerLayout drawerLayout;
        NavigationView navigationView;
        LinearLayout layoutProgress;
        int userRole;

        LinearLayout LayoutInsightData;
        TextView textViewConsumed;
        TextView textViewExpected;
        TextView textViewOverused;
        TextView textViewInsights;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            Log.Debug(TAG, "google app id: " + Resource.String.google_app_id);

            SetContentView(Resource.Layout.Main);
            //msgText = FindViewById<TextView>(Resource.Id.msgText);
            SetDrawer();
            if (Intent.Extras != null)
            {
                foreach (var key in Intent.Extras.KeySet())
                {
                    //int value = Intent.Extras.GetInt(key);
                    //Log.Debug(TAG, "Key: {0} Value: {1}", key, value);

                    if(key.Equals(KEY_USER_ROLE))
                    {
                        userRole = Intent.Extras.GetInt(key);
                    }

                }
            }

            layoutProgress = FindViewById<LinearLayout>(Resource.Id.layout_progress);
            layoutProgress.Visibility = ViewStates.Gone;
            IsPlayServicesAvailable();
            
            if (userRole == (int)Constants.USER_ROLE.ADMIN)
            {
                bool isNetworkEnabled = Utils.Utils.IsNetworkEnabled(this);

                LayoutInsightData = FindViewById<LinearLayout>(Resource.Id.layout_insight_data);
                textViewConsumed = FindViewById<TextView>(Resource.Id.tv_top_consumed);
                textViewExpected = FindViewById<TextView>(Resource.Id.tv_top_expected);
                textViewOverused = FindViewById<TextView>(Resource.Id.tv_top_overused);
                textViewInsights = FindViewById<TextView>(Resource.Id.tv_insights);

                textViewInsights.Click += delegate
                {
                    // Show insights(Recommendations) Activity
                    Intent intent = new Intent(Application.Context, typeof(InsightsActivity));
                    StartActivity(intent);
                };

                if (!isNetworkEnabled)
                {
                    ShowToast("Please enable your internet connection !");
                }
                //Show Map Fragment
                GoogleMapOptions mapOptions = new GoogleMapOptions()
                .InvokeMapType(GoogleMap.MapTypeSatellite)
                .InvokeZoomControlsEnabled(false)
                .InvokeCompassEnabled(true);

                _myMapFragment = MapFragment.NewInstance(mapOptions);
                FragmentTransaction tx = FragmentManager.BeginTransaction();
                tx.Add(Resource.Id.fragment_container, _myMapFragment, "map");
                tx.Commit();

                _myMapFragment.GetMapAsync(this);

                var preferenceHandler = new PreferenceHandler();
                int userId = preferenceHandler.GetUserDetails().User_Id;
                if (userId != -1)
                {
                   if (isNetworkEnabled)
                    {
                        GetMeterDetails(userId);
                        GetMonthlyConsumptionDetails(userId);
                        ShowInsights(null);
                        GetInsights(userId);
                    }
                 }
                else
                {
                    ShowToast("Invalid User Id. Please Login Again !");
                }
            } else
            {
                //Show Student Fragment

                var newFragment = new StudentFragment();
                var ft = FragmentManager.BeginTransaction();
                ft.Add(Resource.Id.fragment_container, newFragment);
                ft.Commit();
            }
            
        }

        private void SetDrawer()
        {
            //var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            //SetSupportActionBar(toolbar);

            //Enable support action bar to display hamburger
            SupportActionBar.SetHomeAsUpIndicator(Resource.Drawable.ic_menu);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);

            var preferenceHandler = new PreferenceHandler();
            bool isLoggedIn = preferenceHandler.IsLoggedIn();
            if (isLoggedIn)
            {
                int roleId = preferenceHandler.GetUserDetails().Role_Id;
                if (roleId == (int)CSU_PORTABLE.Utils.Constants.USER_ROLE.STUDENT)
                {
                    IMenu nav_Menu = navigationView.Menu;
                    nav_Menu.FindItem(Resource.Id.nav_alerts).SetVisible(false);
                }
            }

            TextView textViewUserName = 
                navigationView.GetHeaderView(0).FindViewById<TextView>(
                    Resource.Id.textViewUserName);
            PreferenceHandler pref = new PreferenceHandler();
            UserDetails user = pref.GetUserDetails();
            textViewUserName.Text = user.First_Name + " " + user.Last_Name;

            TextView textViewLogout =
                 navigationView.GetHeaderView(0).FindViewById<TextView>(
                Resource.Id.tv_logout);
            textViewLogout.Click += delegate {
                Logout(new LogoutModel(pref.GetUserDetails().Email));
            };

            navigationView.NavigationItemSelected += (sender, e) => {
                e.MenuItem.SetChecked(true);
                
                //react to click here and swap fragments or navigate
                switch(e.MenuItem.ItemId)
                {
                    case Resource.Id.nav_dashboard:
                        break;
                    case Resource.Id.nav_reports:
                        StartActivity(new Intent(Application.Context, typeof(GlobalReportsActivity)));
                        break;
                    case Resource.Id.nav_alerts:
                        StartActivity(new Intent(Application.Context, typeof(AlertsActivity)));
                        break;
                    case Resource.Id.nav_insights:
                        Intent intent = new Intent(Application.Context, typeof(InsightsActivity));
                        StartActivity(intent);
                        break;
                    case Resource.Id.nav_change_password:
                        StartActivity(new Intent(Application.Context, typeof(ChangePasswordActivity)));
                        break;
                }
                
                drawerLayout.CloseDrawers();
            };
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    drawerLayout.OpenDrawer(Android.Support.V4.View.GravityCompat.Start);
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        private void GetInsights(int userId)
        {
            RestClient client = new RestClient(Constants.SERVER_BASE_URL);
            Log.Debug(TAG, "GetInsights()");

            var request = new RestRequest(Constants.API_GET_INSIGHT_DATA + "/" + userId, Method.GET);

            client.ExecuteAsync(request, response =>
            {
                Console.WriteLine(response);
                if (response.StatusCode != 0)
                {
                    Log.Debug(TAG, "async Response : " + response.ToString());
                    RunOnUiThread(() => {
                        GetInsightDataResponse((RestResponse)response);
                    });
                }
            });
        }

        private void GetInsightDataResponse(RestResponse restResponse)
        {
            if (restResponse != null && restResponse.StatusCode == System.Net.HttpStatusCode.OK && restResponse.Content != null)
            {
                Log.Debug(TAG, restResponse.Content.ToString());
                InshghtDataModel response = JsonConvert.DeserializeObject<InshghtDataModel>(restResponse.Content);


                ShowInsights(response);
            }
            else
            {
                Log.Debug(TAG, "Login Failed");
                ShowInsights(null);
            }
        }

        private void ShowInsights(InshghtDataModel response)
        {
            if (response == null)
            {
                LayoutInsightData.Visibility = ViewStates.Gone;
            } else
            {

                LayoutInsightData.Visibility = ViewStates.Visible;

                textViewConsumed.Text = "" + response.ConsumptionValue;
                textViewExpected.Text = "" + response.PredictedValue;
                float ovr = response.ConsumptionValue - response.PredictedValue;
                textViewOverused.Text = "" + ((ovr < 0 ? 0 : ovr));
            }
        }

        private void GetMonthlyConsumptionDetails(int userId)
        {
            RestClient client = new RestClient(Constants.SERVER_BASE_URL);
            Log.Debug(TAG, "getMeterDetails()");

            var request = new RestRequest(Constants.API_GET_MONTHLY_CONSUMPTION + "/" + userId, Method.GET);

            client.ExecuteAsync(request, response =>
            {
                Console.WriteLine(response);
                if (response.StatusCode != 0)
                {
                    Log.Debug(TAG, "async Response : " + response.ToString());
                    RunOnUiThread(() => {
                        GetMonthlyConsumptionResponse((RestResponse)response);
                    });
                }
            });
        }

        private void GetMeterDetails(int userId)
        {
            RestClient client = new RestClient(Constants.SERVER_BASE_URL);
            Log.Debug(TAG, "getMeterDetails()");

            var request = new RestRequest(Constants.API_GET_METER_LIST + "/" + userId, Method.GET);
            
            client.ExecuteAsync(request, response =>
            {
                Console.WriteLine(response);
                if (response.StatusCode != 0)
                {
                    Log.Debug(TAG, "async Response : " + response.ToString());
                    RunOnUiThread(() => {
                        GetMeterDetailsResponse((RestResponse)response);
                    });
                }
            });
        }

        private void GetMeterDetailsResponse(RestResponse restResponse)
        {
            if (restResponse != null && restResponse.StatusCode == System.Net.HttpStatusCode.OK && restResponse.Content != null)
            {
                Log.Debug(TAG, restResponse.Content.ToString());

                JArray array = JArray.Parse(restResponse.Content);
                meterList = array.ToObject<List<MeterDetails>>();

                addPinAndCircle();
            }
            else
            {
                Log.Debug(TAG, "GetMeterDetailsResponse() Failed");
                ShowToast("GetMeterDetailsResponse() Failed");
            }
        }

        private void GetMonthlyConsumptionResponse(RestResponse restResponse)
        {
            if (restResponse != null && restResponse.StatusCode == System.Net.HttpStatusCode.OK && restResponse.Content != null)
            {
                Log.Debug(TAG, restResponse.Content.ToString());

                JArray array = JArray.Parse(restResponse.Content);
                monthlyConsumptionList = array.ToObject<List<MonthlyConsumptionDetails>>();

                addPinAndCircle();
            }
            else
            {
                Log.Debug(TAG, "GetMonthlyConsumptionResponse() Failed");
                ShowToast("GetMonthlyConsumptionResponse() Failed");
            }
        }

        private void addPinAndCircle()
        {
            if (meterList != null && monthlyConsumptionList != null && IsMapReady)
            {

                for (int i = 0; i < meterList.Count; i++)
                {
                    var meter = meterList[i];
                    LatLng location = new LatLng(meter.Latitude, meter.Longitude);

                    CircleOptions circleOptions = new CircleOptions();
                    circleOptions.InvokeCenter(location);
                    circleOptions.InvokeRadius(getRadius(meter));
                    circleOptions.InvokeFillColor(getColor(meter.Serial));
                    circleOptions.InvokeStrokeColor(getColor(meter.Serial));
                    circleOptions.InvokeStrokeWidth(0);
                    map.AddCircle(circleOptions);

                    MarkerOptions markerOpt1 = new MarkerOptions();
                    markerOpt1.SetPosition(location);
                    markerOpt1.SetTitle(meter.Name);
                    markerOpt1.SetSnippet(meter.Serial);

                    map.AddMarker(markerOpt1);

                    /*CameraPosition.Builder builder = CameraPosition.InvokeBuilder();
                    builder.Target(location);
                    builder.Zoom(18);
                    builder.Bearing(0);
                    builder.Tilt(50);
                    CameraPosition cameraPosition = builder.Build();
                    CameraUpdate cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);

                    map.MoveCamera(cameraUpdate);
                    */
                }
                map.InfoWindowClick += MapOnInfoWindowClick;
            }
        }

        private double getRadius(MeterDetails meter)
        {
            double Monthly_KWH_Consumption = 0;

            for (int i = 0; i < monthlyConsumptionList.Count; i++)
            {
                if (monthlyConsumptionList[i].Powerscout.Equals(meter.Serial))
                {
                    Monthly_KWH_Consumption = monthlyConsumptionList[i].Monthly_KWH_Consumption;
                    break;
                }
            }

            double radius = 0;
            if (Monthly_KWH_Consumption == 0)
            {
                radius = 2;
            }
            else if (Monthly_KWH_Consumption > 0 && Monthly_KWH_Consumption <= 1000)
            {
                if (Monthly_KWH_Consumption < 500)
                {
                    //Minimum radius for the circle
                    radius = 10;
                }
                else
                {
                    radius = Monthly_KWH_Consumption / 50;
                }
            }
            else if (Monthly_KWH_Consumption > 1000 && Monthly_KWH_Consumption <= 10000)
            {
                if (Monthly_KWH_Consumption < 5250)
                {
                    //Minimum radius for the circle
                    radius = 21;
                }
                else
                {
                    radius = Monthly_KWH_Consumption / 250;
                }
            }
            else if (Monthly_KWH_Consumption > 10000 && Monthly_KWH_Consumption <= 38000)
            {
                if (Monthly_KWH_Consumption < 25625)
                {
                    //Minimum radius for the circle
                    radius = 41;
                }
                else
                {
                    radius = Monthly_KWH_Consumption / 625;
                }
            }
            else
            {
                if (Monthly_KWH_Consumption < 61000)
                {
                    //Minimum radius for the circle
                    radius = 61;
                }
                else
                {
                    radius = Monthly_KWH_Consumption / 1000;
                }
            }
            return radius;
        }

        private int getColor(string meterName)
        {
            int fillColor = 0X773CA2E0;//2000462560;
            if (meterName.Equals("P371602077"))
            {
                fillColor = 0X778AD4EB;
            }
            else if (meterName.Equals("P371602079"))
            {
                fillColor = 0X77FE9666;
            }
            else if (meterName.Equals("P371602073"))
            {
                fillColor = 0X77F2C80F;
            }
            else if (meterName.Equals("P371602072"))
            {
                fillColor = 0X77FD625E;
            }
            else if (meterName.Equals("P371602070"))
            {
                fillColor = 0X773CA2E0;
            }
            else if (meterName.Equals("P371602075"))
            {
                fillColor = 0X775F6B6D;
            }
            return fillColor;
        }

        public bool IsPlayServicesAvailable()
        {
            int resultCode = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(this);
            if (resultCode != ConnectionResult.Success)
            {
                if (GoogleApiAvailability.Instance.IsUserResolvableError(resultCode))
                {
                    //msgText.Text = GoogleApiAvailability.Instance.GetErrorString(resultCode);
                    string message = GoogleApiAvailability.Instance.GetErrorString(resultCode);
                    ShowToast(message);
                }
                else
                {
                    string message = "This device is not supported";
                    ShowToast(message);
                    Finish();
                }
                return false;
            }
            else
            {
                string message = "Google Play Services is available.";
                //ShowToast(message);
                return true;
            }
        }

        public void OnMapReady(GoogleMap map)
        {
            this.map = map;
            map.MapType = GoogleMap.MapTypeSatellite;

            LatLng location = new LatLng(40.571276, -105.085522);
            CameraPosition.Builder builder = CameraPosition.InvokeBuilder();
            builder.Target(location);
            builder.Zoom(18);
            builder.Bearing(0);
            builder.Tilt(50);
            CameraPosition cameraPosition = builder.Build();
            CameraUpdate cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);

            IsMapReady = true;
            map.MoveCamera(cameraUpdate);
            addPinAndCircle();
        }

        private bool SetupMapIfNeeded()
        {
            if (map == null)
            {
                _myMapFragment.GetMapAsync(this);
                return false;
            }
            return true;
        }


        private void MapOnInfoWindowClick(object sender, GoogleMap.InfoWindowClickEventArgs e)
        {
            Marker myMarker = e.Marker;

            // Do something with marker.
            Intent intent = new Intent(Application.Context, typeof(MeterReportActivity));
            intent.PutExtra(MeterReportActivity.KEY_METER_NAME, myMarker.Title);
            intent.PutExtra(MeterReportActivity.KEY_METER_SERIAL, myMarker.Snippet);
            StartActivity(intent);
        }

        private void ShowToast(string message)
        {
            if (toast != null)
            {
                toast.Cancel();
            }
            toast = Toast.MakeText(this, message, ToastLength.Short);
            toast.Show();
        }

        private void Logout(LogoutModel logoutModel)
        {
            RestClient client = new RestClient(Constants.SERVER_BASE_URL);
            Log.Debug(TAG, "Logout() " + logoutModel.ToString());

            var request = new RestRequest(Constants.API_SIGN_OUT, Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(logoutModel);
            
            layoutProgress.Visibility = ViewStates.Visible;
            //RestResponse restResponse = (RestResponse)client.Execute(request);
            //LoginResponse(restResponse);
            client.ExecuteAsync(request, response =>
            {
                Console.WriteLine(response);
                if (response.StatusCode != 0)
                {
                    Log.Debug(TAG, "async Response : " + response.ToString());
                    RunOnUiThread(() => {
                        LogoutResponse((RestResponse)response);
                    });
                }
            });
        }

        private void LogoutResponse(RestResponse restResponse)
        {
            if (restResponse != null && restResponse.StatusCode == System.Net.HttpStatusCode.OK && restResponse.Content != null)
            {
                Log.Debug(TAG, restResponse.Content.ToString());
                GeneralResponseModel response = JsonConvert.DeserializeObject<GeneralResponseModel>(restResponse.Content);

                if (response.Status_Code == Constants.STATUS_CODE_SUCCESS)
                {
                    Log.Debug(TAG, "Logout Successful");
                    PreferenceHandler preferenceHandler = new PreferenceHandler();
                    preferenceHandler.setLoggedIn(false);
                    layoutProgress.Visibility = ViewStates.Gone;
                    Finish();
                    StartActivity(new Intent(Application.Context, typeof(LoginActivity)));
                }
                else
                {
                    Log.Debug(TAG, "Logout Failed");
                    layoutProgress.Visibility = ViewStates.Gone;
                    ShowToast("Failed to logout, Please try later.");
                }
            }
            else
            {
                Log.Debug(TAG, "Logout Failed");
                layoutProgress.Visibility = ViewStates.Gone;
                ShowToast("Failed to logout, Please try later.");
            }
        }
    }
}

