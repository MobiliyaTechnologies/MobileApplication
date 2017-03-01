using CSU_PORTABLE.Models;
using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace CSU_PORTABLE.iOS.Utils
{
    class PreferenceHandler
    {
        string preferenceName = "CSUPREF";
        string preferenceIsLoggedIn = "CSUPREF_isLoggedIn";
        string preferenceEmail = "CSUPREF_email";
        string preferenceFirstName = "CSUPREF_first_name";
        string preferenceLastName = "CSUPREF_last_name";
        string preferenceUserId = "CSUPREF_user_id";
        string preferenceRoleId = "CSUPREF_role_id";
        string PreferenceUnreadNotificationCount = "CSUPREF_unread_notifications";

        public bool IsLoggedIn()
        {
            var plist = NSUserDefaults.StandardUserDefaults;
            return plist.BoolForKey(preferenceIsLoggedIn);
        }

        public bool setLoggedIn(bool value)
        {
            var plist = NSUserDefaults.StandardUserDefaults;
            plist.SetBool(value, preferenceIsLoggedIn);
            return plist.Synchronize();
        }

        public bool SaveUserDetails(UserDetails loginResponse)
        {
            var plist = NSUserDefaults.StandardUserDefaults;
            plist.SetString(loginResponse.Email, preferenceEmail);
            plist.SetString(loginResponse.First_Name, preferenceFirstName);
            plist.SetString(loginResponse.Last_Name, preferenceLastName);
            plist.SetInt(loginResponse.User_Id, preferenceUserId);
            plist.SetInt(loginResponse.Role_Id, preferenceRoleId);
            plist.SetBool(true, preferenceIsLoggedIn);
            return plist.Synchronize();
        }

        public UserDetails GetUserDetails()
        {
            UserDetails userDetails = new UserDetails();
            var plist = NSUserDefaults.StandardUserDefaults;

            userDetails.Email = plist.StringForKey(preferenceEmail);
            userDetails.First_Name = plist.StringForKey(preferenceFirstName);
            userDetails.Last_Name = plist.StringForKey(preferenceLastName);
            userDetails.User_Id = (int)plist.IntForKey(preferenceUserId);
            userDetails.Role_Id = (int)plist.IntForKey(preferenceRoleId);
            return userDetails;
        }
    }
}