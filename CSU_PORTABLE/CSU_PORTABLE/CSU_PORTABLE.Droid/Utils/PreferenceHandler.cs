using Android.Content;
using CSU_PORTABLE.Models;
using CSU_PORTABLE.Utils;

namespace CSU_PORTABLE.Droid.Utils
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
        string preferenceToken = "CSUPREF_token";
        string preferenceAccessCode = "CSUPREF_accesscode";


        public bool SetAccessCode(string AccessCode)
        {
            var contextPref = Android.App.Application.Context.GetSharedPreferences(preferenceName, FileCreationMode.Private);
            var contextEdit = contextPref.Edit();
            contextEdit.PutString(preferenceAccessCode, AccessCode);
            return contextEdit.Commit();
        }


        public string GetAccessCode()
        {
            var contextPref = Android.App.Application.Context.GetSharedPreferences(preferenceName, FileCreationMode.Private);
            return contextPref.GetString(preferenceAccessCode, string.Empty);
        }

        public bool SetToken(string Token)
        {
            var contextPref = Android.App.Application.Context.GetSharedPreferences(preferenceName, FileCreationMode.Private);
            var contextEdit = contextPref.Edit();
            contextEdit.PutString(preferenceToken, Token);
            return contextEdit.Commit();
        }


        public string GetToken()
        {
            var contextPref = Android.App.Application.Context.GetSharedPreferences(preferenceName, FileCreationMode.Private);
            return contextPref.GetString(preferenceToken, string.Empty);
        }

        public bool setLoggedIn(bool value)
        {
            var contextPref = Android.App.Application.Context.GetSharedPreferences(preferenceName, FileCreationMode.Private);
            var contextEdit = contextPref.Edit();
            contextEdit.PutBoolean(preferenceIsLoggedIn, value);
            return contextEdit.Commit();
        }

        public bool IsLoggedIn()
        {
            var contextPref = Android.App.Application.Context.GetSharedPreferences(preferenceName, FileCreationMode.Private);
            return contextPref.GetBoolean(preferenceIsLoggedIn, false);
        }

        public bool setUnreadNotificationCount(int value)
        {
            var contextPref = Android.App.Application.Context.GetSharedPreferences(preferenceName, FileCreationMode.Private);
            var contextEdit = contextPref.Edit();
            contextEdit.PutInt(PreferenceUnreadNotificationCount, value);
            return contextEdit.Commit();
        }

        public int getUnreadNotificationCount()
        {
            var contextPref = Android.App.Application.Context.GetSharedPreferences(preferenceName, FileCreationMode.Private);
            return contextPref.GetInt(PreferenceUnreadNotificationCount, 0);
        }

        public bool SaveUserDetails(UserDetails loginResponse)
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

        public UserDetails GetUserDetails()
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