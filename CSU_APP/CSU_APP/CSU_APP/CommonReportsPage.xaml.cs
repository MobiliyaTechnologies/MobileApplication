using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace CSU_APP
{
    public partial class CommonReportsPage : ContentPage
    {

        private WebView webView;
        string t;

        public CommonReportsPage()
        {
            InitializeComponent();

            Init();
        }

        private void Init()
        {

            this.Title ="CSU Reports";
            AuthenticationManager auth_manager = AuthenticationManager.Instance;
            webView = new WebView();
            AuthenticationManager.AccessTokenResponse token = auth_manager.getAccessTokenContent();
            //string html = auth_manager.getBIReportContent();
            ///////////////////////////

            t = token.tokens.AccessToken;

            string html = "<html>Loading Reports</html>";
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

            getMeterReports();
        }

        private void getMeterReports()
        {
            string html = "<html>CSU Reports\n" +
                "\n\n<script type=\"text/javascript\"> " +

                "\n\nvar tileEmbedURL1 = \"https://app.powerbi.com/embed?dashboardId=37f666c4-8a0b-4b5e-bf61-6151f38dae34&tileId=817c8e3b-e17b-4a6a-8530-6272835b4374\"; " +
                "\n\nvar tileEmbedURL2 = \"https://app.powerbi.com/embed?dashboardId=37f666c4-8a0b-4b5e-bf61-6151f38dae34&tileId=1ae30b7e-6577-4c5f-8611-905f9a579899\"; " +
                "\n\nvar iframe; " +
                "\n\nvar accessToken = " + "\'" + t + "\'" + " ; " +
                "\n\nvar height=200;" +
                "\n\nvar width=300; " +

                "function loadEmbededTiles() \n\n{ " +
                    "embedTile1(); " +
                    "embedTile2();" +
                "} " +

                //Weather----------------------------------------

                "function embedTile1() { " +
                    "var embedTileUrl = tileEmbedURL1; " +
                    "if (\"\" === embedTileUrl) { " +
                        "console.log(\"No embed URL found\"); " +
                        "document.getElementById('IFrameEmbedTile1').style.display = 'none'; " +
                        "return; " +
                    "} " +
                    "iframe = document.getElementById('IFrameEmbedTile1'); " +
                    "iframe.src = embedTileUrl + \"&width=\" + width + \"&height=\" + height; " +
                    "iframe.onload = postActionLoadIFrameEmbedTile1; " +
                "} " +

                "function postActionLoadIFrameEmbedTile1() { " +
                    "if (\"\" === accessToken) { " +
                        "console.log(\"Access token not found\"); " +
                        "return; " +
                    "} " +
                    "var h = height; " +
                    "var w = width; " +
                    "var m = { action: \"loadTile\", accessToken: accessToken, height: h, width: w }; " +
                    "var message = JSON.stringify(m); " +
                    "iframe = document.getElementById('IFrameEmbedTile1'); " +
                    "iframe.contentWindow.postMessage(message, \"*\");; " +
                "} " +

                //MonthlyKWh----------------------------------------

                "function embedTile2() { " +
                    "var embedTileUrl = tileEmbedURL2; " +
                    "if (\"\" === embedTileUrl) { " +
                        "console.log(\"No embed URL found\"); " +
                        "document.getElementById('IFrameEmbedTile2').style.display = 'none'; " +
                        "return; " +
                    "} " +
                    "iframe = document.getElementById('IFrameEmbedTile2'); " +
                    "iframe.src = embedTileUrl + \"&width=\" + width + \"&height=\" + height; " +
                    "iframe.onload = postActionLoadIFrameEmbedTile2; " +
                "} " +

                "function postActionLoadIFrameEmbedTile2() { " +
                    "if (\"\" === accessToken) { " +
                        "console.log(\"Access token not found\"); " +
                        "return; " +
                    "} " +
                    "var h = height; " +
                    "var w = width; " +
                    "var m = { action: \"loadTile\", accessToken: accessToken, height: h, width: w }; " +
                    "var message = JSON.stringify(m); " +
                    "iframe = document.getElementById('IFrameEmbedTile2'); " +
                    "iframe.contentWindow.postMessage(message, \"*\");; " +
                "} " +

                //--------------------------------------

                "</script> " +

                "<body " + "onload=\"loadEmbededTiles()\">" +
                "<iframe id=\"IFrameEmbedTile1\" src=\"\" style=\"height:40vh;width:300px\" frameborder=\"0\" seamless></iframe>" +
                "<iframe id=\"IFrameEmbedTile2\" src=\"\" style=\"height:40vh;width:300px;\" frameborder=\"0\" seamless></iframe> " +
                "</body> " +
                "\n\n</html>\n";

            Debug.WriteLine("Html Content : " + html);

            var htmlSourse = new HtmlWebViewSource
            {
                Html = html
            };

            webView.Source = htmlSourse;

        }
    }
}
