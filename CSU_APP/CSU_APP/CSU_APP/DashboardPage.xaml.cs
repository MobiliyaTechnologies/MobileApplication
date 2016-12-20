using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using Xamarin.Forms;

using Xamarin.Forms;

namespace CSU_APP
{
    public partial class DashboardPage : ContentPage
    {
        public DashboardPage()
        {
            InitializeComponent();



            AuthenticationManager auth_manager = AuthenticationManager.Instance;
            var webView = new WebView();
            AuthenticationManager.AccessTokenResponse token = auth_manager.getAccessTokenContent();
            //string html = auth_manager.getBIReportContent();
            ///////////////////////////

            string t = token.tokens.AccessToken;
            string html = "<html> \n\n<script type=\"text/javascript\"> \n\nvar tileEmbedURL = \"https://app.powerbi.com/embed?dashboardId=37f666c4-8a0b-4b5e-bf61-6151f38dae34&tileId=1ae30b7e-6577-4c5f-8611-905f9a579899\"; \n\nvar iframe; \nvar embedTileUrl = \"https://app.powerbi.com/embed?dashboardId=37f666c4-8a0b-4b5e-bf61-6151f38dae34&tileId=817c8e3b-e17b-4a6a-8530-6272835b4374\"; \n\nvar accessToken = " + "\'" + t + "\'" + " ; \n\nfunction embedWeatherTile() \n\n{ \n\nembedTile(); \n\n\n\nif (\"\" === embedTileUrl) \n\n{ console.log(\"No embed URL found\"); \n\nreturn; \n\n} \n\niframe = document.getElementById('weatherIFrame'); \n\niframe.src = embedTileUrl + \"&width=\" + 500 + \"&height=\" + 300; \n\niframe.onload = postActionWeatherLoadTile; \n\n} \n\nfunction postActionWeatherLoadTile() { \n\n// get the access token. \n\n//accessToken = access_Token; \n\n\n\nif (\"\" === accessToken) { \n\nconsole.log(\"Access token not found\"); \n\nreturn; \n\n} \n\nvar h = 200; \n\nvar w = 300; \n\n// construct the push message structure \n\nvar m = { action: \"loadTile\", accessToken: accessToken, height: h, width: w }; \n\nvar message = JSON.stringify(m); \n\n// push the message. \n\niframe = document.getElementById('weatherIFrame'); \n\niframe.contentWindow.postMessage(message, \"*\"); \n\n} \n\nvar height=200; \n\nvar width=300; \n\nfunction embedTile() { \n\n// check if the embed url was selected \n\nvar embedTileUrl = tileEmbedURL; \n\nif (\"\" === embedTileUrl) { \n\nconsole.log(\"No embed URL found\"); \n\nreturn; \n\n} \n\n\n\n// to load a tile do the following: \n\n// 1: set the url, include size. \n\n// 2: add a onload handler to submit the auth token \n\niframe = document.getElementById('iFrameEmbedTile'); \n\niframe.src = embedTileUrl + \"&width=\" + width + \"&height=\" + height; \n\niframe.onload = postActionLoadTile; \n\n} \n\n\n\n\n\n// post the auth token to the iFrame. \n\nfunction postActionLoadTile() { \n\n// get the access token. \n\n\n\n//accessToken = access_Token; \n\n\n\n// return if no a \n\nif (\"\" === accessToken) { \n\nconsole.log(\"Access token not found\"); \n\nreturn; \n\n} \n\n\n\nvar h = height; \n\nvar w = width; \n\n\n\n// construct the push message structure \n\nvar m = { action: \"loadTile\", accessToken: accessToken, height: h, width: w }; \n\nvar message = JSON.stringify(m); \n\n\n\n// push the message. \n\niframe = document.getElementById('iFrameEmbedTile'); \n\niframe.contentWindow.postMessage(message, \"*\");; \n\n} \n\n\n\n\n\n\n\n</script> \n\n\n\n<body onload=\"embedWeatherTile()\"><iframe ID=\"iFrameEmbedTile\" src=\"\" style=\"height:40vh;width:300px\" frameborder=\"0\" seamless></iframe> <iframe id=\"weatherIFrame\" src=\"\" style=\"height:40vh;width:300px;\" frameborder=\"0\" seamless></iframe></body> \n\n</html>\n";


            //////////////////////////////
            Debug.WriteLine("Html Content : " + html);

            var htmlSourse = new HtmlWebViewSource
            {
                Html = html
            };

            webView.Source = htmlSourse;
            Content = webView;

            webView.Navigating += (sender, e) => {
                IsBusy = true;
            };
            webView.Navigated += (sender, e) => {
                IsBusy = false;
            };
            //ToolbarItem tbi = new ToolbarItem();
            //tbi.Text = "Logout";
            //tbi.Clicked += tbi_Clicked;
            //this.ToolbarItems.Add(tbi);

            ToolbarItem setting = new ToolbarItem();
            setting.Text = "Settings";
            setting.Clicked += settingClicked;
            this.ToolbarItems.Add(setting);
        }

        async void tbi_Clicked(object sender, EventArgs e)
        {
            IsBusy = true;
            AuthenticationManager manager = AuthenticationManager.Instance;
            bool responseObj = await manager.performInternalLogOut();
            if (responseObj)
            {
                IsBusy = false;
                //await DisplayAlert("Alert", "Failed to logout", "OK");
                await Application.Current.MainPage.Navigation.PopAsync();
            }
            else
            {
                IsBusy = false;
                await DisplayAlert("Alert", "Failed to logout", "OK");
            }
        }

        async void settingClicked(object sender, EventArgs e)
        {
            await Application.Current.MainPage.Navigation.PushAsync(new SettingPage());
        }
    }
}
