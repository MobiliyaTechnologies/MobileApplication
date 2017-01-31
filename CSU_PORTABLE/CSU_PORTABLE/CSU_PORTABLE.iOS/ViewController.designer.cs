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
    [Register ("ViewController")]
    partial class ViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton ButtonForgotPassword { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton ButtonLogin { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField TextFieldPassword { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField TextFieldUsername { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (ButtonForgotPassword != null) {
                ButtonForgotPassword.Dispose ();
                ButtonForgotPassword = null;
            }

            if (ButtonLogin != null) {
                ButtonLogin.Dispose ();
                ButtonLogin = null;
            }

            if (TextFieldPassword != null) {
                TextFieldPassword.Dispose ();
                TextFieldPassword = null;
            }

            if (TextFieldUsername != null) {
                TextFieldUsername.Dispose ();
                TextFieldUsername = null;
            }
        }
    }
}