using CSU_PORTABLE.iOS.Utils;
using Foundation;
using UIKit;
using Firebase.Analytics;
using UserNotifications;
using System;
using Firebase.CloudMessaging;
using Firebase.InstanceID;
using BigTed;
using CSU_PORTABLE.Models;
using CSU_PORTABLE.Utils;

namespace CSU_PORTABLE.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the
    // User Interface of the application, as well as listening (and optionally responding) to application events from iOS.
    [Register("AppDelegate")]
    public class AppDelegate : UIApplicationDelegate, IUNUserNotificationCenterDelegate, IMessagingDelegate
    {
        // class-level declarations

        public override UIWindow Window
        {
            get;
            set;
        }

        public RootViewController RootViewController { get { return Window.RootViewController as RootViewController; } }

        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            // Override point for customization after application launch.
            // If not required for your application you can safely delete this method


            //PreferenceHandler preferenceHandler = new PreferenceHandler();
            //UIStoryboard storyBoard = UIStoryboard.FromName("Main", null);
            //UIViewController vc;
            //if (preferenceHandler.IsLoggedIn())
            //{
            //    vc = storyBoard.InstantiateViewController("ViewController") as ViewController;
            //    //vc = storyBoard.InstantiateViewController("MapViewController") as MapViewController;

            //}
            //else
            //{
            //    vc = storyBoard.InstantiateViewController("ViewController") as ViewController;
            //}
            //this.Window.RootViewController = new UINavigationController(vc);
            //// set our root view controller with the sidebar menu as the apps root view controller
            Window = new UIWindow(UIScreen.MainScreen.Bounds);
            Window.RootViewController = new RootViewController();
            this.Window.MakeKeyAndVisible();

            UIApplication.SharedApplication.RegisterForRemoteNotifications();


            //FCM integration start
            App.Configure();

            // Register your app for remote notifications.
            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
            {
                // iOS 10 or later
                var authOptions = UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound;
                UNUserNotificationCenter.Current.RequestAuthorization(authOptions, (granted, error) =>
                {
                    Console.WriteLine(granted);
                });

                // For iOS 10 display notification (sent via APNS)
                UNUserNotificationCenter.Current.Delegate = this;

                // For iOS 10 data message (sent via FCM)
                Messaging.SharedInstance.RemoteMessageDelegate = this;
            }
            else
            {
                // iOS 9 or before
                var allNotificationTypes = UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound;
                var settings = UIUserNotificationSettings.GetSettingsForTypes(allNotificationTypes, null);
                UIApplication.SharedApplication.RegisterUserNotificationSettings(settings);
            }

            UIApplication.SharedApplication.RegisterForRemoteNotifications();

            Messaging.SharedInstance.Connect(error =>
            {
                if (error != null)
                {
                    // Handle if something went wrong while connecting
                    Console.WriteLine("FCM Connect error: " + error);
                }
                else
                {
                    // Let the user know that connection was successful
                    Console.WriteLine("FCM Connection successful");
                }
            });

            //var token = InstanceId.SharedInstance.Token;

            // Monitor token generation
            InstanceId.Notifications.ObserveTokenRefresh((sender, e) =>
            {
                // Note that this callback will be fired everytime a new token is generated, including the first
                // time. So if you need to retrieve the token as soon as it is available this is where that
                // should be done.
                var refreshedToken = InstanceId.SharedInstance.Token;

                // Do your magic to refresh the token where is needed
                if (Constants.IsDemoMode)
                {
                    Messaging.SharedInstance.Subscribe("AlertsDemo");
                }
                else
                {
                    Messaging.SharedInstance.Subscribe("Alerts");
                }
            });

            // Request notification permissions from the user
            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
            {
                try
                {
                    UNUserNotificationCenter.Current.RequestAuthorization(UNAuthorizationOptions.Alert, (approved, err) =>
                    {
                        // Handle approval
                    });
                }
                catch (Exception)
                {
                }
            }
            //FCM integration end

            return true;
        }

        public override void OnResignActivation(UIApplication application)
        {
            // Invoked when the application is about to move from active to inactive state.
            // This can occur for certain types of temporary interruptions (such as an incoming phone call or SMS message) 
            // or when the user quits the application and it begins the transition to the background state.
            // Games should use this method to pause the game.
        }

        public override void DidEnterBackground(UIApplication application)
        {
            // Use this method to release shared resources, save user data, invalidate timers and store the application state.
            // If your application supports background exection this method is called instead of WillTerminate when the user quits.

            //FCM
            Messaging.SharedInstance.Disconnect();
            Console.WriteLine("Disconnected from FCM");
        }

        public override void WillEnterForeground(UIApplication application)
        {
            // Called as part of the transiton from background to active state.
            // Here you can undo many of the changes made on entering the background.
        }

        public override void OnActivated(UIApplication application)
        {
            // Restart any tasks that were paused (or not yet started) while the application was inactive. 
            // If the application was previously in the background, optionally refresh the user interface.
        }

        public override void WillTerminate(UIApplication application)
        {
            // Called when the application is about to terminate. Save data, if needed. See also DidEnterBackground.
        }

        //FCM Start
        // To receive notifications in foregroung on iOS 9 and below.
        // To receive notifications in background in any iOS version
        public override void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler)
        {
            // If you are receiving a notification message while your app is in the background,
            // this callback will not be fired 'till the user taps on the notification launching the application.

            // Do your magic to handle the notification data
            UIApplication.SharedApplication.ApplicationIconBadgeNumber++;
            System.Console.WriteLine(userInfo);
        }

        // To receive notifications in foreground on iOS 10 devices.
        [Export("userNotificationCenter:willPresentNotification:withCompletionHandler:")]
        public void WillPresentNotification(UNUserNotificationCenter center, UNNotification notification, Action<UNNotificationPresentationOptions> completionHandler)
        {
            // Do your magic to handle the notification data
            Console.WriteLine("WillPresentNotification gaurav 1 val");

            //PreferenceHandler prefsHandler = new PreferenceHandler();
            if (PreferenceHandler.IsLoggedIn())
            {
                int roleId = PreferenceHandler.GetUserDetails().RoleId;
                if (roleId == (int)CSU_PORTABLE.Utils.Constants.USER_ROLE.ADMIN)
                {
                    BTProgressHUD.ForceiOS6LookAndFeel = true;
                    BTProgressHUD.ShowToast("New Alert Received !", true, 2000.0);
                }
            }
            //UIApplication.SharedApplication.ApplicationIconBadgeNumber++;
            //UserDetails userDT = prefsHandler.GetUserDetails();

            //UIApplication.SharedApplication.ApplicationIconBadgeNumber = UIApplication.SharedApplication.ApplicationIconBadgeNumber + 1;

        }

        // Receive data message on iOS 10 devices.
        public void ApplicationReceivedRemoteMessage(RemoteMessage remoteMessage)
        {
            Console.WriteLine(remoteMessage.AppData);
            Console.WriteLine("ApplicationReceivedRemoteMessage gaurav 1 val");
        }

        [Export("userNotificationCenter:didReceiveNotificationResponse:withCompletionHandler:")]
        public void DidReceiveNotificationResponse(UNUserNotificationCenter center, UNNotificationResponse response, Action completionHandler)
        {
            // Do your magic to handle the notification data
            System.Console.WriteLine(response.Notification.Request.Content.UserInfo);
            Console.WriteLine("DidReceiveNotificationResponse gaurav 1 val");
        }
        //FCM End


        public override void ReceivedLocalNotification(UIApplication application, UILocalNotification notification)
        {
            base.ReceivedLocalNotification(application, notification);
            UIApplication.SharedApplication.ApplicationIconBadgeNumber++;
        }
    }
}


