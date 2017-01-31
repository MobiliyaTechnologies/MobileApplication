// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace CSU_PORTABLE.iOS
{
    [Register ("MyMenuController")]
    partial class MyMenuController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton AlertsButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton ChangePasswordButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton LogOutButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView ProfileBack { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton ProfileName { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton ReportsButton { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (AlertsButton != null) {
                AlertsButton.Dispose ();
                AlertsButton = null;
            }

            if (ChangePasswordButton != null) {
                ChangePasswordButton.Dispose ();
                ChangePasswordButton = null;
            }

            if (LogOutButton != null) {
                LogOutButton.Dispose ();
                LogOutButton = null;
            }

            if (ProfileBack != null) {
                ProfileBack.Dispose ();
                ProfileBack = null;
            }

            if (ProfileName != null) {
                ProfileName.Dispose ();
                ProfileName = null;
            }

            if (ReportsButton != null) {
                ReportsButton.Dispose ();
                ReportsButton = null;
            }
        }
    }
}