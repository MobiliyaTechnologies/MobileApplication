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
    [Register ("MenuController")]
    partial class MenuController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton ContentButton { get; set; }

        [Action ("ContentButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ContentButton_TouchUpInside (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (ContentButton != null) {
                ContentButton.Dispose ();
                ContentButton = null;
            }
        }
    }
}