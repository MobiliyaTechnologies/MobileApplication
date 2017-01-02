using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace CSU_APP
{
    public partial class App : Application
    {
        public static double ScreenHeight;
        public static double ScreenWidth;

        public App()
        {
            InitializeComponent();

            //MainPage = new CSU_APP.MainPage();
            //MainPage = new NavigationPage(new MS_POCPage());

            //MainPage = new NavigationPage(new DashboardPage());
            
            var preferenceHandler = DependencyService.Get<IPreferencesHandler>();
            if (preferenceHandler != null)
            {
                if (preferenceHandler.IsLoggedIn())
                {
                    MainPage = new NavigationPage(new MapPage());
                    //MainPage = new NavigationPage(new MyMapPage());
                } else
                {
                    MainPage = new NavigationPage(new MS_POCPage());
                }
            } else
            {
                MainPage = new NavigationPage(new MS_POCPage());

            }
            
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
