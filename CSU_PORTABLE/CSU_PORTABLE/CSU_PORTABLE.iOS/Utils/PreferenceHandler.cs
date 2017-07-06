using CSU_PORTABLE.Models;
using Foundation;
using Newtonsoft.Json;
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
        string preferenceUserCampus = "CSUPREF_user_campus";
        string PreferenceUnreadNotificationCount = "CSUPREF_unread_notifications";
        string preferenceToken = "CSUPREF_token";
        string preferenceAccessCode = "CSUPREF_accesscode";
        string preferenceConfig = "CSUPREF_config";
        string preferenceDomainKey = "CSUPREF_domainkey";


        public void SetDomainKey(string domainKey)
        {
            var plist = NSUserDefaults.StandardUserDefaults;
            plist.SetString(domainKey, preferenceDomainKey);
            plist.Synchronize();
        }


        public string GetDomainKey()
        {
            var plist = NSUserDefaults.StandardUserDefaults;
            return plist.StringForKey(preferenceDomainKey);
        }

        public void SetConfig(string Config)
        {
            var plist = NSUserDefaults.StandardUserDefaults;
            plist.SetString(Config, preferenceConfig);
            plist.Synchronize();
        }


        public string GetConfig()
        {
            var plist = NSUserDefaults.StandardUserDefaults;
            return plist.StringForKey(preferenceConfig);
        }

        public void SetAccessCode(string AccessCode)
        {
            var plist = NSUserDefaults.StandardUserDefaults;
            plist.SetString(AccessCode, preferenceAccessCode);
            plist.Synchronize();
        }


        public string GetAccessCode()
        {
            var plist = NSUserDefaults.StandardUserDefaults;
            return plist.StringForKey(preferenceAccessCode);
        }

        public void SetToken(string Token)
        {
            var plist = NSUserDefaults.StandardUserDefaults;
            plist.SetString(Token, preferenceToken);
            plist.Synchronize();
        }


        public string GetToken()
        {
            var plist = NSUserDefaults.StandardUserDefaults;
            return plist.StringForKey(preferenceToken);
        }


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
            plist.SetString(loginResponse.FirstName, preferenceFirstName);
            plist.SetString(loginResponse.LastName, preferenceLastName);
            plist.SetInt(loginResponse.UserId, preferenceUserId);
            plist.SetInt(loginResponse.RoleId, preferenceRoleId);
            plist.SetBool(true, preferenceIsLoggedIn);
            plist.SetString(JsonConvert.SerializeObject(loginResponse.UserCampus), preferenceUserCampus);
            return plist.Synchronize();
        }

        public UserDetails GetUserDetails()
        {
            UserDetails userDetails = new UserDetails();
            var plist = NSUserDefaults.StandardUserDefaults;

            userDetails.Email = plist.StringForKey(preferenceEmail);
            userDetails.FirstName = plist.StringForKey(preferenceFirstName);
            userDetails.LastName = plist.StringForKey(preferenceLastName);
            userDetails.UserId = (int)plist.IntForKey(preferenceUserId);
            userDetails.RoleId = (int)plist.IntForKey(preferenceRoleId);
            string userCampus = plist.StringForKey(preferenceUserCampus);
            if (!string.IsNullOrWhiteSpace(userCampus))
            {
                userDetails.UserCampus = JsonConvert.DeserializeObject<List<CampusModel>>(userCampus);
            }
            return userDetails;
        }
    }
}