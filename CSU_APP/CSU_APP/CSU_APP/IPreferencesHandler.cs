using CSU_APP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSU_APP
{
    public interface IPreferencesHandler
    {
        bool IsLoggedIn();

        bool setLoggedIn(bool value);

        bool SaveUserDetails(UserDetails loginResponse);

        UserDetails GetUserDetails();
    }
}
