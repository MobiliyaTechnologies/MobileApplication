using EM_PORTABLE.Models;
using Foundation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace EM_PORTABLE.iOS.Utils
{
    public static class PreferenceHandler
    {
        
        static string preferenceIsLoggedIn = "EFPREF_isLoggedIn";
        static string preferenceEmail = "EFPREF_email";
        static string preferenceFirstName = "EFPREF_first_name";
        static string preferenceLastName = "EFPREF_last_name";
        static string preferenceUserId = "EFPREF_user_id";
        static string preferenceRoleId = "EFPREF_role_id";
        static string preferenceUserCampus = "EFPREF_user_campus";
        static string preferenceToken = "EFPREF_token";
        static string preferenceRefreshToken = "EFPREF_refreshtoken";
        static string preferenceAccessCode = "EFPREF_accesscode";
        static string preferenceConfig = "EFPREF_config";
        static string preferenceDomainKey = "EFPREF_domainkey";


        public static void SetDomainKey(string domainKey)
        {
            var plist = NSUserDefaults.StandardUserDefaults;
            plist.SetString(domainKey, preferenceDomainKey);
            plist.Synchronize();
        }


        public static string GetDomainKey()
        {
            var plist = NSUserDefaults.StandardUserDefaults;
            return plist.StringForKey(preferenceDomainKey);
        }

        public static void SetConfig(string Config)
        {
            var plist = NSUserDefaults.StandardUserDefaults;
            plist.SetString(Config, preferenceConfig);
            plist.Synchronize();
        }


        public static string GetConfig()
        {
            var plist = NSUserDefaults.StandardUserDefaults;
            return plist.StringForKey(preferenceConfig);
        }

        public static void SetAccessCode(string AccessCode)
        {
            var plist = NSUserDefaults.StandardUserDefaults;
            plist.SetString(AccessCode, preferenceAccessCode);
            plist.Synchronize();
        }


        public static string GetAccessCode()
        {
            var plist = NSUserDefaults.StandardUserDefaults;
            return plist.StringForKey(preferenceAccessCode);
        }

        public static void SetToken(string Token)
        {
            var plist = NSUserDefaults.StandardUserDefaults;
            plist.SetString(Token, preferenceToken);
            plist.Synchronize();
        }


        public static string GetToken()
        {
            var plist = NSUserDefaults.StandardUserDefaults;
            return plist.StringForKey(preferenceToken);
        }

        public static void SetRefreshToken(string RefreshToken)
        {
            var plist = NSUserDefaults.StandardUserDefaults;
            plist.SetString(RefreshToken, preferenceRefreshToken);
            plist.Synchronize();
        }


        public static string GetRefreshToken()
        {
            var plist = NSUserDefaults.StandardUserDefaults;
            return plist.StringForKey(preferenceRefreshToken);
        }


        public static bool IsLoggedIn()
        {
            var plist = NSUserDefaults.StandardUserDefaults;
            return plist.BoolForKey(preferenceIsLoggedIn);
        }

        public static bool setLoggedIn(bool value)
        {
            var plist = NSUserDefaults.StandardUserDefaults;
            plist.SetBool(value, preferenceIsLoggedIn);
            return plist.Synchronize();
        }

        public static bool SaveUserDetails(UserDetails loginResponse)
        {
            var plist = NSUserDefaults.StandardUserDefaults;
            if (loginResponse != null)
            {
                plist.SetString(loginResponse.Email, preferenceEmail);
                plist.SetString(loginResponse.FirstName, preferenceFirstName);
                plist.SetString(loginResponse.LastName, preferenceLastName);
                plist.SetInt(loginResponse.UserId, preferenceUserId);
                plist.SetInt(loginResponse.RoleId, preferenceRoleId);
                plist.SetBool(true, preferenceIsLoggedIn);
                plist.SetString(JsonConvert.SerializeObject(loginResponse.UserCampus), preferenceUserCampus);

            }
            return plist.Synchronize();
        }

        public static UserDetails GetUserDetails()
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