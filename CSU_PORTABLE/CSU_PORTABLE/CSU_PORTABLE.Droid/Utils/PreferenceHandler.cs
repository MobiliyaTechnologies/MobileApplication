using Android.Content;
using CSU_PORTABLE.Models;

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
            return userDetails;
        }
    }
}