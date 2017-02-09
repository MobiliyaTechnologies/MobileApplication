using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSU_PORTABLE.Utils
{
    public class Constants
    {
        public static int STATUS_CODE_SUCCESS = 200;
        public static int STATUS_CODE_FAILED = 0;

        public static string SERVER_BASE_URL = "http://powergridrestservice.azurewebsites.net/api/";
        public static string API_SIGN_IN = "signin";
        public static string API_SIGN_OUT = "signout";
        public static string API_CHANGE_PASSWORD = "changepassword";
        public static string API_FORGOT_PASSWORD = "forgotpassword";
        public static string API_GET_METER_LIST = "getmeterlist";
        public static string API_GET_MONTHLY_CONSUMPTION = "getmonthlyconsumption";
        public static string API_GET_METER_REPORTS = "getpowerbiurl";
        public static string API_GET_GLOBAL_REPORTS = "getpowerbigeneralurl";
        public static string API_GET_CLASS_ROOMS = "getclassrooms";

        public static string SERVER_BASE_URL_FOR_TOKEN = "http://csuwebapp.azurewebsites.net/PowerBIService.asmx/";
        public static string API_GET_TOKEN = "GetAccessToken";
    }
}
