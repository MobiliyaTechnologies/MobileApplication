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
using Android.Support.V7.App;
using Android.Support.V4.Widget;
using Android.Support.V4.View;
using Android.Support.Design.Widget;
using CSU_PORTABLE.Droid.Utils;
using CSU_PORTABLE.Models;
using Android.Gms.Common;
using Android.Support.V7.Widget;
using CSU_PORTABLE.Utils;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Android.Util;
using System.Net;
using static CSU_PORTABLE.Utils.Constants;
using Android.Content.PM;

namespace CSU_PORTABLE.Droid.UI
{
    [Activity(Label = "Dashboard", MainLauncher = false, Icon = "@drawable/icon", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait, Theme = "@style/MyTheme")]
    public class AdminDashboardActivity : AppCompatActivity
    {
        const string TAG = "MainActivity";
        DrawerLayout drawerLayout;
        NavigationView navigationView;
        PreferenceHandler preferenceHandler;
        Activity activityContext;
        LinearLayout layoutProgress;
        RecyclerView mRecyclerView;
        ConsumptionListAdapter mAdapter;
        LinearLayoutManager mLayoutManager;
        ConsumptionFor CurrentConsumption;
        int CurrentBuildingId = 0;
        int CurrentPremisesId = 0;
        List<ConsumptionModel> consumpModels;
        int userRole;
        public static TextView notifCount;
        MySampleBroadcastReceiver receiver;

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.AdminDashboard);
            //activityContext = this;
            preferenceHandler = new PreferenceHandler();
            CurrentConsumption = ConsumptionFor.Premises;
            // Create your application here
            SetDrawer();
            await CreateDashboard();
            IsPlayServicesAvailable();
        }

        private async Task CreateDashboard()
        {
            string token = preferenceHandler.GetToken();
            if (string.IsNullOrEmpty(token))
            {
                string tokenURL = string.Format(B2CConfig.TokenURL, B2CConfig.Tenant, B2CPolicy.SignInPolicyId, B2CConfig.ClientId, preferenceHandler.GetAccessCode());
                //string tokenURL = B2CConfigManager.GetInstance().GetB2CTokenUrl(preferenceHandler.GetAccessCode());
                
                var response = await InvokeApi.Authenticate(tokenURL, string.Empty, HttpMethod.Post);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string strContent = await response.Content.ReadAsStringAsync();
                    var tokenNew = JsonConvert.DeserializeObject<AccessToken>(strContent);
                    preferenceHandler.SetToken(tokenNew.id_token);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest || response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    RedirectToLogin();
                }
            }

            UserDetails user = preferenceHandler.GetUserDetails();
            token = preferenceHandler.GetToken();
            //Get User Details
            if (user.UserId == -1)
            {
                var responseUser = await InvokeApi.Invoke(Constants.API_GET_CURRENTUSER, string.Empty, HttpMethod.Get, token);
                if (responseUser.StatusCode == HttpStatusCode.OK)
                {
                    GetCurrentUserResponse(responseUser);
                    user = preferenceHandler.GetUserDetails();
                }
                else
                {
                    RedirectToLogin();
                }
            }

            // tempaory made admin
            if (user.RoleId == 1)
            {
                GetConsumptionDetails(CurrentConsumption, 0);
            }
            else
            {
                //Show Student Fragment
                var newFragment = new StudentFragment();
                var ft = FragmentManager.BeginTransaction();
                ft.Add(Resource.Id.fragment_container, newFragment);
                ft.Commit();
                layoutProgress = FindViewById<LinearLayout>(Resource.Id.layout_progress);
                layoutProgress.Visibility = ViewStates.Gone;
            }
            //layoutProgress = FindViewById<LinearLayout>(Resource.Id.layout_progress);
            //layoutProgress.Visibility = ViewStates.Gone;

        }

        private async void GetCurrentUserResponse(HttpResponseMessage responseUser)
        {
            if (responseUser != null && responseUser.StatusCode == System.Net.HttpStatusCode.OK && responseUser.Content != null)
            {
                string strContent = await responseUser.Content.ReadAsStringAsync();
                UserDetails user = JsonConvert.DeserializeObject<UserDetails>(strContent);
                var preferenceHandler = new PreferenceHandler();
                preferenceHandler.SaveUserDetails(user);

            }
            else
            {
                Utils.Utils.ShowToast(Application.Context, "User details not found!");
            }
        }

        public override void OnBackPressed()
        {
            //base.OnBackPressed();
            layoutProgress = FindViewById<LinearLayout>(Resource.Id.layout_progress);
            layoutProgress.Visibility = ViewStates.Visible;
            layoutProgress.Enabled = true;
            switch (CurrentConsumption)
            {
                case ConsumptionFor.Buildings:
                    CurrentConsumption = ConsumptionFor.Premises;
                    GetConsumptionDetails(CurrentConsumption, 0);
                    CurrentPremisesId = 0;
                    break;
                case ConsumptionFor.Meters:

                    CurrentConsumption = ConsumptionFor.Buildings;
                    GetConsumptionDetails(CurrentConsumption, CurrentPremisesId);

                    break;
            }

        }

        #region " Consumption "
        private void SetConsumptions(List<ConsumptionModel> consumpModels)
        {
            //List<ConsumptionModel> consumpModels = new List<ConsumptionModel>();
            //consumpModels.Add(new ConsumptionModel()
            //{
            //    Consumed = 1,
            //    Expected = 1,
            //    Id = 1,
            //    Name = "CSU 1 ",
            //    Overused = 2,
            //    Path = ""

            //});
            //consumpModels.Add(new ConsumptionModel()
            //{
            //    Consumed = 23,
            //    Expected = 30,
            //    Id = 1,
            //    Name = "CSU 2",
            //    Overused = 7,
            //    Path = ""

            //});
            //consumpModels.Add(new ConsumptionModel()
            //{
            //    Consumed = 17,
            //    Expected = 22,
            //    Id = 1,
            //    Name = "CSU 3",
            //    Overused = 5,
            //    Path = ""

            //});
            mAdapter = new ConsumptionListAdapter(this, consumpModels);
            mAdapter.ItemClick += OnItemClick;

            // Get our RecyclerView layout:
            mRecyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerViewConsumption);

            mLayoutManager = new LinearLayoutManager(this);
            mRecyclerView.SetLayoutManager(mLayoutManager);
            // Plug the adapter into the RecyclerView:
            mRecyclerView.SetAdapter(mAdapter);


        }

        private async void GetConsumptionDetails(ConsumptionFor currentConsumption, int Id)
        {
            string url = GetConsumptionURL(currentConsumption);
            if (Id != 0)
            {
                url = url + "/" + Convert.ToString(Id);
            }
            var responseConsumption = await InvokeApi.Invoke(url, string.Empty, HttpMethod.Get, preferenceHandler.GetToken());
            if (responseConsumption.StatusCode == HttpStatusCode.OK)
            {
                GetConsumptionResponse(responseConsumption);
                layoutProgress.Visibility = ViewStates.Gone;
            }
            else if (responseConsumption.StatusCode == HttpStatusCode.BadRequest || responseConsumption.StatusCode == HttpStatusCode.Unauthorized)
            {
                RedirectToLogin();
            }
        }

        private string GetConsumptionURL(ConsumptionFor currentConsumption)
        {
            switch (currentConsumption)
            {
                case ConsumptionFor.Premises:
                    return Constants.API_GET_ALLPREMISES;
                case ConsumptionFor.Buildings:
                    return Constants.API_GET_ALLBUILDINGS_BY_PREMISEID;
                case ConsumptionFor.Meters:
                    return Constants.API_GET_ALLMETERS_BY_BUILDINGID;
                default:
                    return string.Empty;
            }
        }


        private List<ConsumptionModel> GetConsumptionModels(string consumptionContent)
        {
            List<ConsumptionModel> conModels = new List<ConsumptionModel>();
            switch (CurrentConsumption)
            {
                case ConsumptionFor.Premises:
                    var modelPremise = JsonConvert.DeserializeObject<List<Premise>>(consumptionContent);
                    conModels = modelPremise.ConvertAll(x => new ConsumptionModel()
                    {
                        Id = x.PremiseID,
                        Name = CurrentConsumption.ToString() + " - " + x.PremiseName,
                        Consumed = Convert.ToString(Math.Round((x.MonthlyConsumption / 1000), 2)) + " K",
                        Expected = Convert.ToString(Math.Round((x.MonthlyPrediction / 1000), 2)) + " K",
                        Overused = Convert.ToString(Math.Round((x.MonthlyPrediction - x.MonthlyConsumption) / 1000, 2)) + " K"
                    });
                    break;
                case ConsumptionFor.Buildings:
                    var modelBuilding = JsonConvert.DeserializeObject<List<Building>>(consumptionContent);
                    return modelBuilding.ConvertAll(x => new ConsumptionModel()
                    {
                        Id = x.BuildingID,
                        Name = CurrentConsumption.ToString() + " - " + x.BuildingName,
                        Consumed = Convert.ToString(Math.Round((x.MonthlyConsumption / 1000), 2)) + " K",
                        Expected = Convert.ToString(Math.Round((x.MonthlyPrediction / 1000), 2)) + " K",
                        Overused = Convert.ToString(Math.Round((x.MonthlyPrediction - x.MonthlyConsumption) / 1000, 2)) + " K"
                    });
                case ConsumptionFor.Meters:
                    var modelMeter = JsonConvert.DeserializeObject<List<Meter>>(consumptionContent);
                    return modelMeter.ConvertAll(x => new ConsumptionModel()
                    {
                        Id = x.Id,
                        Name = CurrentConsumption.ToString() + " - " + x.PowerScout,
                        Consumed = Convert.ToString(Math.Round((x.MonthlyConsumption / 1000), 2)) + " K",
                        Expected = Convert.ToString(Math.Round((x.MonthlyPrediction / 1000), 2)) + " K",
                        Overused = Convert.ToString(Math.Round((x.MonthlyPrediction - x.MonthlyConsumption) / 1000, 2)) + " K"
                    });
            }
            return conModels;
        }


        private async void GetConsumptionResponse(HttpResponseMessage responseConsumption)
        {
            if (responseConsumption != null && responseConsumption.StatusCode == System.Net.HttpStatusCode.OK && responseConsumption.Content != null)
            {
                string strContent = await responseConsumption.Content.ReadAsStringAsync();
                List<ConsumptionModel> consumptions = GetConsumptionModels(strContent);
                //consumpModels = JsonConvert.DeserializeObject<List<ConsumptionModel>>(strContent);
                SetConsumptions(consumptions);
                layoutProgress = FindViewById<LinearLayout>(Resource.Id.layout_progress);
                layoutProgress.Visibility = ViewStates.Gone;
            }
            else
            {
                RedirectToLogin();
            }
        }

        private void RedirectToLogin()
        {
            preferenceHandler.setLoggedIn(false);
            preferenceHandler.SetToken(string.Empty);
            Finish();
            Intent intent = new Intent(Application.Context, typeof(LoginNewActivity));
            intent.PutExtra(LoginNewActivity.KEY_SHOW_PAGE, (int)SignInType.SIGN_IN);
            StartActivity(intent);
            layoutProgress = FindViewById<LinearLayout>(Resource.Id.layout_progress);
            layoutProgress.Visibility = ViewStates.Gone;
        }

        void OnItemClick(object sender, int position)
        {
            if (CurrentConsumption != ConsumptionFor.Meters)
            {
                layoutProgress = FindViewById<LinearLayout>(Resource.Id.layout_progress);
                layoutProgress.Visibility = ViewStates.Visible;
                layoutProgress.Enabled = true;
                int photoNum = position + 1;
                ConsumptionListAdapter consumptionListAdapter = (ConsumptionListAdapter)sender;
                //Toast.MakeText(this, "This is photo number " + photoNum.ToString(), ToastLength.Short).Show();
                switch (CurrentConsumption)
                {
                    case ConsumptionFor.Premises:
                        CurrentPremisesId = consumptionListAdapter.mConsumptionModels[position].Id;
                        CurrentConsumption = ConsumptionFor.Buildings;
                        break;
                    case ConsumptionFor.Buildings:
                        //CurrentBuildingId = consumptionListAdapter.mConsumptionModels[position].Id;
                        CurrentConsumption = ConsumptionFor.Meters;
                        break;
                }
                GetConsumptionDetails(CurrentConsumption, consumptionListAdapter.mConsumptionModels[position].Id);
            }
        }

        #endregion

        #region " Dashboard Menu and Notifications"

        private void SetDrawer()
        {
            //var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            //SetSupportActionBar(toolbar);

            //Enable support action bar to display hamburger
            SupportActionBar.SetHomeAsUpIndicator(Resource.Drawable.ic_menu);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);


            bool isLoggedIn = preferenceHandler.IsLoggedIn();
            if (isLoggedIn)
            {
                int roleId = preferenceHandler.GetUserDetails().RoleId;
                if (roleId == (int)CSU_PORTABLE.Utils.Constants.USER_ROLE.STUDENT)
                {
                    IMenu nav_Menu = navigationView.Menu;
                    nav_Menu.FindItem(Resource.Id.nav_dashboard).SetVisible(false);
                    //nav_Menu.FindItem(Resource.Id.nav_reports).SetVisible(false);
                    nav_Menu.FindItem(Resource.Id.nav_insights).SetVisible(false);
                    nav_Menu.FindItem(Resource.Id.nav_alerts).SetVisible(false);
                }
            }

            TextView textViewUserName =
                navigationView.GetHeaderView(0).FindViewById<TextView>(
                    Resource.Id.textViewUserName);
            PreferenceHandler pref = new PreferenceHandler();
            UserDetails user = pref.GetUserDetails();
            textViewUserName.Text = user.FirstName + " " + user.LastName;

            TextView textViewLogout =
                 navigationView.GetHeaderView(0).FindViewById<TextView>(
                Resource.Id.tv_logout);
            textViewLogout.Click += delegate
            {
                Logout(new LogoutModel(pref.GetUserDetails().Email));
            };

            navigationView.NavigationItemSelected += (sender, e) =>
            {
                e.MenuItem.SetChecked(true);

                //react to click here and swap fragments or navigate
                switch (e.MenuItem.ItemId)
                {
                    case Resource.Id.nav_dashboard:
                        break;
                    //case Resource.Id.nav_reports:
                    //    StartActivity(new Intent(Application.Context, typeof(GlobalReportsActivity)));
                    //    break;
                    case Resource.Id.nav_alerts:
                        showAlerts();
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


        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.main_menu, menu);
            /*if (userRole == (int)Constants.USER_ROLE.STUDENT)
            {
                menu.GetItem(0).SetVisible(false);
            }
            else
            {

                RelativeLayout alertItem = (RelativeLayout)(menu.FindItem(Resource.Id.alerts).ActionView);
                alertItem.Click += delegate
                {
                    showAlerts();
                };
                notifCount = alertItem.FindViewById<TextView>(Resource.Id.notif_count);
                setNotificationCount();

            }*/
            return base.OnPrepareOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.alerts:
                    showAlerts();
                    return true;

                case Android.Resource.Id.Home:
                    drawerLayout.OpenDrawer(GravityCompat.Start);// OPEN DRAWER
                    return true;
            }
            return base.OnOptionsItemSelected(item);
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
                    Utils.Utils.ShowToast(this, message);
                }
                else
                {
                    string message = "This device is not supported";
                    Utils.Utils.ShowToast(this, message);
                    Finish();
                }
                return false;
            }
            else
            {
                string message = "Google Play Services is available.";
                return true;
            }
        }

        //protected override void OnResume()
        //{
        //    base.OnResume();
        //    RegisterReceiver(receiver, new IntentFilter(Utils.Utils.ALERT_BROADCAST));
        //    if (userRole == (int)Constants.USER_ROLE.ADMIN)
        //    {
        //        setNotificationCount();
        //    }
        //}

        //protected override void OnPause()
        //{
        //    UnregisterReceiver(receiver);
        //    // Code omitted for clarity
        //    base.OnPause();
        //}

        public void setNotificationCount()
        {
            if (notifCount == null)
            {
                return;
            }
            PreferenceHandler prefs = new PreferenceHandler();
            int notificationCount = prefs.getUnreadNotificationCount();

            if (notificationCount <= 0)
            {
                notifCount.Visibility = ViewStates.Gone;
            }
            else
            {
                notifCount.Visibility = ViewStates.Visible;
                notifCount.Text = notificationCount.ToString();
            }
        }

        private void showAlerts()
        {
            StartActivity(new Intent(Application.Context, typeof(AlertsActivity)));
        }

        private void Logout(LogoutModel logoutModel)
        {
            Log.Debug(TAG, "Local Logout Started");
            //PreferenceHandler preferenceHandler = new PreferenceHandler();
            preferenceHandler.setLoggedIn(false);
            layoutProgress.Visibility = ViewStates.Gone;
            Finish();
            StartActivity(new Intent(Application.Context, typeof(LoginActivity)));

        }

        private void LogoutResponse(HttpResponseMessage restResponse)
        {

            Log.Debug(TAG, "Local Logout Successful");
            layoutProgress.Visibility = ViewStates.Gone;
            RedirectToLogin();
        }

        [BroadcastReceiver(Enabled = true, Exported = false)]
        [IntentFilter(new[] { Utils.Utils.ALERT_BROADCAST })]
        class MySampleBroadcastReceiver : BroadcastReceiver
        {
            private Activity activityContext;

            public MySampleBroadcastReceiver() { }

            public MySampleBroadcastReceiver(Activity activityContext)
            {
                this.activityContext = activityContext;
            }

            public override void OnReceive(Context context, Intent intent)
            {
                try
                {
                    ((MainActivity)activityContext).setNotificationCount();
                }
                catch (Exception e)
                {

                }
            }
        }
        #endregion
    }
}