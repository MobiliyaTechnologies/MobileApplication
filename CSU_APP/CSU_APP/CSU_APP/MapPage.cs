using CSU_APP.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace CSU_APP
{

    class MapPage : ContentPage
    {

        IList<Models.MeterDetails> meterList = null;
        IList<Models.MonthlyConsumptionDetails> monthlyConsumptionList = null;
        CustomMap map;
        StackLayout stack;
        ActivityIndicator indicator;

        public MapPage()
        {
            this.Title = "CSU";
            indicator = new ActivityIndicator
            {
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                Color = Color.Gray,
                IsVisible = false
            };

            map = new CustomMap(
                MapSpan.FromCenterAndRadius(
                        new Position(40.571276, -105.085522), Distance.FromMiles(0.1)))
            {
                IsShowingUser = true,
                WidthRequest = 960,     // App.ScreenWidth,
                HeightRequest = 100,    // App.ScreenHeight
                VerticalOptions = LayoutOptions.FillAndExpand
            };
            
            map.MapType = MapType.Satellite;

            stack = new StackLayout { Spacing = 0 };
            stack.Children.Add(indicator);
            stack.Children.Add(map);
            Content = stack;

            indicator.IsRunning = true;
            indicator.IsVisible = true;

            var preferenceHandler = DependencyService.Get<IPreferencesHandler>();
            int userId = preferenceHandler.GetUserDetails().User_Id;
            if (userId != -1)
            {
                getMeterDetails(userId);
                getMonthlyConsumptionDetails(userId);
            }

            ToolbarItem setting = new ToolbarItem();
            setting.Text = "Menu";
            setting.Clicked += settingClicked;
            this.ToolbarItems.Add(setting);
        }

        async void settingClicked(object sender, EventArgs e)
        {
            await Application.Current.MainPage.Navigation.PushAsync(new SettingPage());
        }

        private void addPin(CustomMap map, Models.MeterDetails meter)
        {
            var position = new Position(meter.Latitude, meter.Longitude); // Latitude, Longitude
            var pin = new Pin
            {
                Type = PinType.Place,
                Position = position,
                Label = meter.Name,
                Address = meter.Serial
            };

            pin.Clicked += async (sender, args) => {
                await Application.Current.MainPage.Navigation.PushAsync(new DashboardPage(pin.Address, pin.Label));

            };
            map.Pins.Add(pin);
        }

        async private void getMeterDetails(int userId)
        {
            AuthenticationManager manager = AuthenticationManager.Instance;
            meterList = await manager.getMeterDetails(userId);
            if (map == null)
            {
                return;
            }

            addPinAndCircle();
        }

        async private void getMonthlyConsumptionDetails(int userId)
        {
            AuthenticationManager manager = AuthenticationManager.Instance;
            monthlyConsumptionList = await manager.getMonthlyConsumptionDetails(userId);
            if (map == null)
            {
                return;
            }

            addPinAndCircle();
        }

        private void addPinAndCircle()
        {
            
            if (meterList != null && monthlyConsumptionList != null)
            {

                indicator.IsRunning = false;
                indicator.IsVisible = false;

                List<CustomCircle> cList = new List<CustomCircle>();
                for (int i = 0; i < meterList.Count; i++)
                {
                    var meter = meterList[i];
                    CustomCircle circle1 = new CustomCircle();
                    circle1.Position = new Position(meter.Latitude, meter.Longitude);
                    circle1.Radius = getRadius(meter);
                    circle1.Color = meter.Serial;//getColor(meter);
                    cList.Add(circle1);
                }
                map.CircleList = cList;

                for (int i = 0; i < meterList.Count; i++)
                {
                    addPin(map, meterList[i]);
                }

                stack = new StackLayout { Spacing = 0 };
                stack.Children.Add(map);
                Content = stack;
            }
        }

        private double getRadius(MeterDetails meter)
        {
            double Monthly_KWH_Consumption = 0;

            for (int i=0; i<monthlyConsumptionList.Count; i++)
            {
                if(monthlyConsumptionList[i].Powerscout.Equals(meter.Serial))
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
    }
}
