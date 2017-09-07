// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace CSU_PORTABLE.iOS
{
    [Register ("ForgotPasswordController")]
    partial class ForgotPasswordController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField Email { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton ForgotButton { get; set; }

        [Action ("ForgotButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ForgotButton_TouchUpInside (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (Email != null) {
                Email.Dispose ();
                Email = null;
            }

            if (ForgotButton != null) {
                ForgotButton.Dispose ();
                ForgotButton = null;
            }
        }
    }
}