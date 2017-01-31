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
    [Register ("ChangePasswordController")]
    partial class ChangePasswordController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField ConfirmPassword { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField Password { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton Submit { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel Username { get; set; }

        [Action ("Submit_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void Submit_TouchUpInside (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (ConfirmPassword != null) {
                ConfirmPassword.Dispose ();
                ConfirmPassword = null;
            }

            if (Password != null) {
                Password.Dispose ();
                Password = null;
            }

            if (Submit != null) {
                Submit.Dispose ();
                Submit = null;
            }

            if (Username != null) {
                Username.Dispose ();
                Username = null;
            }
        }
    }
}