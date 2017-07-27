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
using Android.Webkit;
using Android.Graphics;
using System.IO;
using Android.Content.Res;

namespace CSU_PORTABLE.Droid.UI
{
    [Activity(Label = "ChartsActivity")]
    public class ChartsActivity : Activity
    {
        private WebView localChartView;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Charts);
            localChartView = FindViewById<WebView>(Resource.Id.LocalChartView);
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
            localChartView.LoadDataWithBaseURL("file:///android_asset/", content, "text/html", "utf-8", null);
           
        }



    }
}

public class MyChartView : WebViewClient
{

    public override void OnPageFinished(WebView view, string url)
    {
        base.OnPageFinished(view, url);

    }

    public override void OnPageStarted(WebView view, string url, Bitmap favicon)
    {
        base.OnPageStarted(view, url, favicon);
    }
}