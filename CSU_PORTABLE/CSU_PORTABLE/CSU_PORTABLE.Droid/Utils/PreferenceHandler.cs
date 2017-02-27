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
                contextEdit.PutString(preferenceFirstName, loginResponse.First_Name);
                contextEdit.PutString(preferenceLastName, loginResponse.Last_Name);
                contextEdit.PutInt(preferenceUserId, loginResponse.User_Id);
                contextEdit.PutInt(preferenceRoleId, loginResponse.Role_Id);
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
            userDetails.First_Name = contextPref.GetString(preferenceFirstName, null);
            userDetails.Last_Name = contextPref.GetString(preferenceLastName, null);
            userDetails.User_Id = contextPref.GetInt(preferenceUserId, -1);
            userDetails.Role_Id = contextPref.GetInt(preferenceRoleId, (int)Constants.USER_ROLE.ADMIN);
            return userDetails;
        }
    }
}