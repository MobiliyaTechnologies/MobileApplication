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
using Android.Webkit;
using Android.Graphics;
using UserDetailsClient;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Android.Content.Res;
using System.IO;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using System.Security.Cryptography.X509Certificates;

namespace CSU_PORTABLE.Droid.UI
{
    [Activity(Label = "Dashboard", MainLauncher = false, Icon = "@drawable/icon", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait, Theme = "@style/MyTheme")]
    public class AdminDashboardActivity : AppCompatActivity, IPlatformParameters
    {
        const string TAG = "AdminDashboardActivity";
        DrawerLayout drawerLayout;
        NavigationView navigationView;
        //PreferenceHandler preferenceHandler;
        LinearLayout layoutProgress;
        RecyclerView mRecyclerView;
        ConsumptionListAdapter mAdapter;
        LinearLayoutManager mLayoutManager;
        ConsumptionFor CurrentConsumption;
        int CurrentPremisesId = 0;
        public static TextView notifCount;
        MySampleBroadcastReceiver receiver;
        private WebView localChartView;
        public MqttClient client;

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.AdminDashboard);
            CurrentConsumption = ConsumptionFor.Premises;
            receiver = new MySampleBroadcastReceiver(this);


            if (!Utils.Utils.IsNetworkEnabled(this))
            {
                RunOnUiThread(() =>
                {
                    Utils.Utils.ShowDialog(this, "Internet not available.");
                });
                StartActivity(new Intent(Application.Context, typeof(LoginActivity)));
                Finish();
            }
            else
            {
                if (PreferenceHandler.GetToken() == string.Empty)
                {
                    await Utils.Utils.GetToken();
                }
                CreateDashboard();
                IsPlayServicesAvailable();
            }

        }

        public async Task<AuthenticationResult> Authenticate(Activity context, string authority, string resource, string clientId, string returnUri)
        {
            var authContext = new AuthenticationContext(authority);
            if (authContext.TokenCache.ReadItems().Any())
                authContext = new AuthenticationContext(authContext.TokenCache.ReadItems().First().Authority);
            //var result = await authContext.AcquireDeviceCodeAsync("https://login.windows.net/common", B2CConfig.ClientId);
            var uri = new Uri(returnUri);
            var platformParams = new PlatformParameters(context);
            try
            {
                var authResult = await authContext.AcquireTokenAsync(clientId, clientId, uri, platformParams);
                //var authResult = await authContext.AcquireTokenAsync("https://CSUB2C.onmicrosoft.com/EMTestDeploy", new ClientCredential(B2CConfig.ClientId, B2CConfig.ClientSecret));

                var re = await authContext.AcquireTokenByAuthorizationCodeAsync(authResult.AccessToken, uri, new ClientCredential(B2CConfig.ClientId, B2CConfig.ClientSecret));
                return authResult;
            }
            catch (AdalException)
            {
                return null;
            }
        }

        private void CreateDashboard()
        {
            try
            {
                if (Utils.Utils.CurrentStage == DemoStage.None && IsDemoMode)
                {
                    Utils.Utils.CurrentStage = DemoStage.Yesterday;
                }
                SubscribeMQTT(this);
            }
            catch (Exception)
            {

            }
            SetDrawer();
            if (PreferenceHandler.GetUserDetails().RoleId == (int)USER_ROLE.ADMIN)
            {
                GetConsumptionDetails(CurrentConsumption, 0);
                this.Title = "Dashboard";
            }
            else
            {
                //Show Student Fragment
                this.Title = "Feedback";
                var newFragment = new StudentFragment();
                var ft = FragmentManager.BeginTransaction();
                ft.Add(Resource.Id.fragment_container, newFragment);
                ft.Commit();
                layoutProgress = FindViewById<LinearLayout>(Resource.Id.layout_progress);
                layoutProgress.Visibility = ViewStates.Gone;
            }


        }

        #region " Bar Chart "

        private void SetConsumptionBarChartWebView(List<ConsumptionModel> consumpModels)
        {
            string[][] r = Array.ConvertAll(consumpModels.Select(x => new { x.Name, x.Consumed, x.Expected, x.Overused }).ToArray(), x => new string[] { x.Name, x.Consumed, x.Expected, x.Overused });
            string[] Labels = null;
            try
            {
                Labels = consumpModels.Select(x => x.Name.Split('-')[1]).ToArray();
            }
            catch (Exception ex)
            {
                Labels = consumpModels.Select(x => x.Name).ToArray();
            }
            string[] Consumed = consumpModels.Select(x => x.Consumed.Replace('K', ' ').Trim()).ToArray();
            string[] Expected = consumpModels.Select(x => x.Expected.Replace('K', ' ').Trim()).ToArray();
            string[] Overused = consumpModels.Select(x => x.Overused.Replace('K', ' ').Trim()).ToArray();
            localChartView = FindViewById<WebView>(Resource.Id.LocalChartView1);
            localChartView.SetWebViewClient(new MyChartView()); // stops request going to Web Browser
            localChartView.ClearCache(true);
            localChartView.RequestFocusFromTouch();
            localChartView.Settings.JavaScriptEnabled = true;
            string content = string.Empty;
            AssetManager aManager = this.Assets;
            using (StreamReader sr = new StreamReader(aManager.Open("ChartC3.html")))
            {
                content = sr.ReadToEnd();
            }
            content = content.Replace("LabelsData", "'" + string.Join("','", Labels) + "'");
            content = content.Replace("ConsumedData", "'" + string.Join("','", Consumed) + "'");
            content = content.Replace("ExpectedData", "'" + string.Join("','", Expected) + "'");
            content = content.Replace("OverusedData", "'" + string.Join("','", Overused) + "'");
            //content = content.Replace("jsondata", "[['Premises', 'Consumed', 'Expected', 'Overused'],['Premise 1', 1000, 400, 200],['Premise 1', 1170, 460, 230],['Premise 1', 660, 1120, 240],['Premise 1', 1030, 540, 260]]");
            localChartView.LoadDataWithBaseURL("file:///android_asset/", content, "text/html", "utf-8", null);
            //localChartView.LoadUrl("file:///android_asset/ChartC3.html");
        }

        #endregion

        private async void GetCurrentUserResponse(HttpResponseMessage responseUser)
        {
            if (responseUser != null && responseUser.StatusCode == System.Net.HttpStatusCode.OK && responseUser.Content != null)
            {
                string strContent = await responseUser.Content.ReadAsStringAsync();
                UserDetails user = JsonConvert.DeserializeObject<UserDetails>(strContent);
                PreferenceHandler.SaveUserDetails(user);
                CreateDashboard();
            }
        }

        public async Task GetUserDetails()
        {
            var responseUser = await InvokeApi.Invoke(Constants.API_GET_CURRENTUSER, string.Empty, HttpMethod.Get, PreferenceHandler.GetToken());
            if (responseUser.StatusCode == HttpStatusCode.OK)
            {
                GetCurrentUserResponse(responseUser);
            }
            else if (responseUser.StatusCode == HttpStatusCode.Unauthorized)
            {
                await Utils.Utils.RefreshToken(this);
            }
        }


        #region " Consumption "

        public override void OnBackPressed()
        {
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
                case ConsumptionFor.Premises:
                    this.CloseContextMenu();
                    layoutProgress.Visibility = ViewStates.Gone;
                    break;
            }

        }

        private void SetConsumptions(List<ConsumptionModel> consumpModels)
        {
            //SetConsumptionBarChart(consumpModels);
            SetConsumptionBarChartWebView(consumpModels);
            mAdapter = new ConsumptionListAdapter(this, consumpModels);
            mAdapter.ItemClick += OnItemClick;

            // Get our RecyclerView layout:
            mRecyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerViewConsumption);

            mLayoutManager = new LinearLayoutManager(this);
            mRecyclerView.SetLayoutManager(mLayoutManager);
            mRecyclerView.SetAdapter(mAdapter);
        }

        private async void GetConsumptionDetails(ConsumptionFor currentConsumption, int Id)
        {
            string url = GetConsumptionURL(currentConsumption);
            if (Id != 0)
            {
                url = url + "/" + Convert.ToString(Id);
            }
            var responseConsumption = await InvokeApi.Invoke(url, string.Empty, HttpMethod.Get, PreferenceHandler.GetToken(), Utils.Utils.CurrentStage);
            if (responseConsumption.StatusCode == HttpStatusCode.OK)
            {
                GetConsumptionResponse(responseConsumption);
                layoutProgress.Visibility = ViewStates.Gone;
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

                if (consumptions.Count > 0)
                {
                    SetConsumptions(consumptions);
                }
                else
                {
                    Utils.Utils.ShowDialog(this, "No " + CurrentConsumption.ToString() + " found!");
                    switch (CurrentConsumption)
                    {
                        case ConsumptionFor.Buildings:
                            CurrentConsumption = ConsumptionFor.Premises;
                            break;
                        case ConsumptionFor.Meters:
                            CurrentConsumption = ConsumptionFor.Buildings;
                            break;
                    }
                }

                layoutProgress = FindViewById<LinearLayout>(Resource.Id.layout_progress);
                layoutProgress.Visibility = ViewStates.Gone;
            }
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
                switch (CurrentConsumption)
                {
                    case ConsumptionFor.Premises:
                        CurrentPremisesId = consumptionListAdapter.mConsumptionModels[position].Id;
                        CurrentConsumption = ConsumptionFor.Buildings;
                        break;
                    case ConsumptionFor.Buildings:
                        CurrentConsumption = ConsumptionFor.Meters;
                        break;
                }
                GetConsumptionDetails(CurrentConsumption, consumptionListAdapter.mConsumptionModels[position].Id);
            }
        }


        private void GetCurrentConsumption(int position)
        {
            if (CurrentConsumption != ConsumptionFor.Meters)
            {
                layoutProgress = FindViewById<LinearLayout>(Resource.Id.layout_progress);
                layoutProgress.Visibility = ViewStates.Visible;
                layoutProgress.Enabled = true;
                switch (CurrentConsumption)
                {
                    case ConsumptionFor.Premises:
                        CurrentPremisesId = position;
                        CurrentConsumption = ConsumptionFor.Buildings;
                        break;
                    case ConsumptionFor.Buildings:
                        CurrentConsumption = ConsumptionFor.Meters;
                        break;
                }
                GetConsumptionDetails(CurrentConsumption, position);
            }
        }
        #endregion

        #region " Dashboard Menu and Notifications"

        private void SetDrawer()
        {
            SupportActionBar.SetHomeAsUpIndicator(Resource.Drawable.ic_menu);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            bool isLoggedIn = PreferenceHandler.IsLoggedIn();
            if (isLoggedIn)
            {
                int roleId = PreferenceHandler.GetUserDetails().RoleId;
                if (roleId == (int)CSU_PORTABLE.Utils.Constants.USER_ROLE.STUDENT)
                {
                    IMenu nav_Menu = navigationView.Menu;
                    nav_Menu.FindItem(Resource.Id.nav_dashboard).SetVisible(false);
                    nav_Menu.FindItem(Resource.Id.nav_insights).SetVisible(false);
                    nav_Menu.FindItem(Resource.Id.nav_alerts).SetVisible(false);
                }
            }

            TextView textViewUserName =
                navigationView.GetHeaderView(0).FindViewById<TextView>(
                    Resource.Id.textViewUserName);
            UserDetails user = PreferenceHandler.GetUserDetails();
            textViewUserName.Text = user.FirstName + " " + user.LastName;

            TextView textViewLogout =
                 navigationView.GetHeaderView(0).FindViewById<TextView>(
                Resource.Id.tv_logout);
            textViewLogout.Click += delegate
            {
                Logout(new LogoutModel(PreferenceHandler.GetUserDetails().Email));
            };

            navigationView.NavigationItemSelected += (sender, e) =>
            {
                e.MenuItem.SetChecked(true);
                //react to click here and swap fragments or navigate
                switch (e.MenuItem.ItemId)
                {
                    case Resource.Id.nav_dashboard:
                        break;
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

            //if (optionsMenu != null)
            //{
            //    MenuInflater.Inflate(Resource.Menu.main_menu, optionsMenu);

            //    if (PreferenceHandler.GetUserDetails().RoleId == (int)Constants.USER_ROLE.STUDENT)
            //    {
            //        optionsMenu.GetItem(0).SetVisible(false);
            //    }
            //    else
            //    {
            //        RelativeLayout alertItem = (RelativeLayout)(optionsMenu.FindItem(Resource.Id.alerts).ActionView);
            //        alertItem.Click += delegate
            //        {
            //            showAlerts();
            //        };
            //        notifCount = alertItem.FindViewById<TextView>(Resource.Id.notif_count);
            //        setNotificationCount();
            //    }
            //}
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            //optionsMenu = menu;
            MenuInflater.Inflate(Resource.Menu.main_menu, menu);
            if (PreferenceHandler.GetUserDetails().RoleId == (int)Constants.USER_ROLE.STUDENT)
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

            }
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
                // string message = "Google Play Services is available.";
                return true;
            }
        }

        protected override void OnResume()
        {
            base.OnResume();
            RegisterReceiver(receiver, new IntentFilter(Utils.Utils.ALERT_BROADCAST));
            if (PreferenceHandler.GetUserDetails().RoleId == (int)Constants.USER_ROLE.ADMIN)
            {
                setNotificationCount();
            }
        }

        protected override void OnPause()
        {
            UnregisterReceiver(receiver);
            // Code omitted for clarity
            base.OnPause();
        }

        public void setNotificationCount()
        {
            if (notifCount == null)
            {
                return;
            }
            //PreferenceHandler prefs = new PreferenceHandler();
            int notificationCount = PreferenceHandler.getUnreadNotificationCount();

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
            layoutProgress.Visibility = ViewStates.Visible;
            Log.Debug(TAG, "Local Logout Started");
            PreferenceHandler.setLoggedIn(false);
            PreferenceHandler.SetToken(string.Empty);
            PreferenceHandler.SetRefreshToken(string.Empty);
            PreferenceHandler.SaveUserDetails(new UserDetails());
            CookieManager.Instance.RemoveAllCookie();
            StartActivity(new Intent(Application.Context, typeof(LoginActivity)));
            Finish();
            layoutProgress.Visibility = ViewStates.Gone;
        }

        private void LogoutResponse(HttpResponseMessage restResponse)
        {
            Log.Debug(TAG, "Local Logout Successful");
            layoutProgress.Visibility = ViewStates.Gone;
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
                catch (Exception)
                {

                }
            }
        }
        #endregion

        #region " MQTT "

        public void SubscribeMQTT(Context context)
        {
            if (client == null)
            {
                client = new MqttClient(Constants.MqttServer);
            }
            if (client != null && client.IsConnected == false)
            {
                byte code = client.Connect(Guid.NewGuid().ToString());
                string[] topics = Constants.MqttTopics;
                client.Subscribe(topics, new byte[] { 0 });
                client.MqttMsgPublishReceived += new MqttClient.MqttMsgPublishEventHandler(client_MqttMsgPublishReceived);
            }
        }

        public void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            RunOnUiThread(() =>
            {
                if (!string.IsNullOrEmpty(Encoding.UTF8.GetString(e.Message)))
                {
                    try
                    {
                        layoutProgress.Visibility = ViewStates.Visible;
                        int stage = Convert.ToInt32(JsonConvert.DeserializeObject<DemoState>(Encoding.UTF8.GetString(e.Message)).State);
                        Utils.Utils.CurrentStage = (DemoStage)stage;
                        if (PreferenceHandler.GetUserDetails().RoleId == (int)USER_ROLE.ADMIN)
                        {
                            StartActivity(new Intent(Application.Context, typeof(AdminDashboardActivity)));
                            Finish();
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            });

            //Utils.Utils.ShowDialog(this, CurrentStage.ToString());
        }

        //[Service]
        //public class MQTT
        //{
        //    Context localContext;
        //    DelegateShowMessage ShowMainMessage;
        //    public void SubscribeMQTT(Context context)
        //    {
        //        this.localContext = context;
        //        if (client == null)
        //        {
        //            client = new MqttClient("54.165.217.177");

        //        }
        //        if (client != null && client.IsConnected == false)
        //        {
        //            byte code = client.Connect(Guid.NewGuid().ToString());
        //            string[] topics = { "krunal1" };
        //            client.Subscribe(topics, new byte[] { 0 });
        //            client.MqttMsgPublishReceived += new MqttClient.MqttMsgPublishEventHandler(client_MqttMsgPublishReceived);
        //        }
        //    }

        //    public delegate void MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e);
        //    public void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        //    {

        //    }

        //    private void Client_MqttMsgSubscribed(object sender, MqttMsgSubscribedEventArgs e)
        //    {
        //        client.MqttMsgPublishReceived += Client_MqttMsgPublishReceived;
        //    }

        //    private void Client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        //    {
        //        throw new NotImplementedException();
        //    }

        //}
        #endregion

    }
}