using Android.Content;
using CSU_PORTABLE.Models;
using CSU_PORTABLE.Utils;

namespace CSU_PORTABLE.Droid.Utils
{
    public static class PreferenceHandler
    {
        static string preferenceName = "CSUPREF";
        static string preferenceIsLoggedIn = "CSUPREF_isLoggedIn";
        static string preferenceEmail = "CSUPREF_email";
        static string preferenceFirstName = "CSUPREF_first_name";
        static string preferenceLastName = "CSUPREF_last_name";
        static string preferenceUserId = "CSUPREF_user_id";
        static string preferenceRoleId = "CSUPREF_role_id";
        static string PreferenceUnreadNotificationCount = "CSUPREF_unread_notifications";
        static string preferenceToken = "CSUPREF_token";
        static string preferenceAccessCode = "CSUPREF_accesscode";
        static string preferenceConfig = "CSUPREF_config";
        static string preferenceDomainKey = "CSUPREF_domainkey";

        public static bool SetDomainKey(string domainKey)
        {
            var contextPref = Android.App.Application.Context.GetSharedPreferences(preferenceName, FileCreationMode.Private);
            var contextEdit = contextPref.Edit();
            contextEdit.PutString(preferenceDomainKey, domainKey);
            return contextEdit.Commit();
        }


        public static string GetDomainKey()
        {
            var contextPref = Android.App.Application.Context.GetSharedPreferences(preferenceName, FileCreationMode.Private);
            return contextPref.GetString(preferenceDomainKey, string.Empty);
        }


        public static bool SetConfig(string Config)
        {
            var contextPref = Android.App.Application.Context.GetSharedPreferences(preferenceName, FileCreationMode.Private);
            var contextEdit = contextPref.Edit();
            contextEdit.PutString(preferenceConfig, Config);
            return contextEdit.Commit();
        }


        public static string GetConfig()
        {
            var contextPref = Android.App.Application.Context.GetSharedPreferences(preferenceName, FileCreationMode.Private);
            return contextPref.GetString(preferenceConfig, string.Empty);
        }

        public static bool SetAccessCode(string AccessCode)
        {
            var contextPref = Android.App.Application.Context.GetSharedPreferences(preferenceName, FileCreationMode.Private);
            var contextEdit = contextPref.Edit();
            contextEdit.PutString(preferenceAccessCode, AccessCode);
            return contextEdit.Commit();
        }


        public static string GetAccessCode()
        {
            var contextPref = Android.App.Application.Context.GetSharedPreferences(preferenceName, FileCreationMode.Private);
            return contextPref.GetString(preferenceAccessCode, string.Empty);
        }

        public static bool SetToken(string Token)
        {
            var contextPref = Android.App.Application.Context.GetSharedPreferences(preferenceName, FileCreationMode.Private);
            var contextEdit = contextPref.Edit();
            contextEdit.PutString(preferenceToken, Token);
            return contextEdit.Commit();
        }


        public static string GetToken()
        {
            var contextPref = Android.App.Application.Context.GetSharedPreferences(preferenceName, FileCreationMode.Private);
            return contextPref.GetString(preferenceToken, string.Empty);
        }

        public static bool setLoggedIn(bool value)
        {
            var contextPref = Android.App.Application.Context.GetSharedPreferences(preferenceName, FileCreationMode.Private);
            var contextEdit = contextPref.Edit();
            contextEdit.PutBoolean(preferenceIsLoggedIn, value);
            return contextEdit.Commit();
        }

        public static bool IsLoggedIn()
        {
            var contextPref = Android.App.Application.Context.GetSharedPreferences(preferenceName, FileCreationMode.Private);
            return contextPref.GetBoolean(preferenceIsLoggedIn, false);
        }

        public static bool setUnreadNotificationCount(int value)
        {
            var contextPref = Android.App.Application.Context.GetSharedPreferences(preferenceName, FileCreationMode.Private);
            var contextEdit = contextPref.Edit();
            contextEdit.PutInt(PreferenceUnreadNotificationCount, value);
            return contextEdit.Commit();
        }

        public static int getUnreadNotificationCount()
        {
            var contextPref = Android.App.Application.Context.GetSharedPreferences(preferenceName, FileCreationMode.Private);
            return contextPref.GetInt(PreferenceUnreadNotificationCount, 0);
        }

        public static bool SaveUserDetails(UserDetails loginResponse)
        {
            bool resp = false;
            if (loginResponse != null)
            {
                var contextPref = Android.App.Application.Context.GetSharedPreferences(preferenceName, FileCreationMode.Private);
                var contextEdit = contextPref.Edit();
                contextEdit.PutString(preferenceEmail, loginResponse.Email);
                contextEdit.PutString(preferenceFirstName, loginResponse.FirstName);
                contextEdit.PutString(preferenceLastName, loginResponse.LastName);
                contextEdit.PutInt(preferenceUserId, loginResponse.UserId);
                contextEdit.PutInt(preferenceRoleId, loginResponse.RoleId);
                contextEdit.PutBoolean(preferenceIsLoggedIn, true);
                resp = contextEdit.Commit();
            }
            return resp;
        }

        public static UserDetails GetUserDetails()
        {
            UserDetails userDetails = new UserDetails();
            var contextPref = Android.App.Application.Context.GetSharedPreferences(preferenceName, FileCreationMode.Private);
            userDetails.Email = contextPref.GetString(preferenceEmail, null);
            userDetails.FirstName = contextPref.GetString(preferenceFirstName, null);
            userDetails.LastName = contextPref.GetString(preferenceLastName, null);
            userDetails.UserId = contextPref.GetInt(preferenceUserId, -1);
            userDetails.RoleId = contextPref.GetInt(preferenceRoleId, (int)Constants.USER_ROLE.ADMIN);
            return userDetails;
        }
    }
}