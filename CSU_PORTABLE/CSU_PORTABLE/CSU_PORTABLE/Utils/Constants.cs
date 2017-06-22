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

        public static string SERVER_BASE_URL = "http://energymanagementapi.azurewebsites.net/api/";
        //public static string SERVER_BASE_URL = "http://powergridrestservice.azurewebsites.net/api/";
        //public static string SERVER_BASE_URL = "http://emrestdemodeploy123.azurewebsites.net/api/";
        public static string API_SIGN_IN = "signin";
        public static string API_SIGN_OUT = "signout";
        public static string API_CHANGE_PASSWORD = "changepassword";
        public static string API_FORGOT_PASSWORD = "forgotpassword";
        public static string API_GET_METER_LIST = "getmeterlist";
        public static string API_GET_MONTHLY_CONSUMPTION = "GetMonthWiseConsumption";
        public static string API_GET_METER_REPORTS = "getpowerbiurl";
        public static string API_GET_GLOBAL_REPORTS = "getpowerbigeneralurl";
        public static string API_GET_CLASS_ROOMS = "getclassrooms";
        public static string API_GET_ALL_ALERTS = "getallalerts";
        public static string API_ACKNOWLWDGE_ALERTS = "acknowledgealert";
        public static string API_GET_QUESTION_ANSWERS = "getquestionanswers";
        public static string API_GIVE_FEEDBACK = "storefeedback";
        public static string API_GET_INSIGHT_DATA = "getinsightdata";
        public static string API_GET_RECOMMENDATIONS = "getrecommendations";
        public static string API_GET_CURRENTUSER = "GetCurrentUserWithPremise";
        public static string API_GET_BUILDINGSBYCAMPUS = "GetBuildingsByCampus";
        public static string API_GET_ALLPREMISES = "GetAllPremise";
        public static string API_GET_ALLBUILDINGS_BY_PREMISEID = "GetBuildingsByPremise";
        public static string API_GET_ALLMETERS_BY_BUILDINGID = "GetMeterList";

        //public static string SERVER_BASE_URL_FOR_TOKEN = "http://csuwebapp.azurewebsites.net/PowerBIService.asmx/";
        public static string SERVER_BASE_URL_FOR_TOKEN = "http://13.72.102.73/CSU/Powerbiservice.asmx";
        public static string API_GET_TOKEN = "GetAccessToken";

        public enum USER_ROLE
        {
            ADMIN = 1,
            STUDENT = 2
        }

        public enum SignInType
        {
            SIGN_IN = 1,
            SIGN_UP = 2
        }

    }

    public enum ConsumptionFor
    {
        Premises = 1,
        Buildings = 2,
        Meters = 3
    };


    public class AccessToken
    {
        public string id_token { get; set; }
        public string token_type { get; set; }
        public string not_before { get; set; }
        public string id_token_expires_in { get; set; }
        public string profile_info { get; set; }

    }

    public class B2CPolicy
    {
        public static string SignUpPolicyId = "B2C_1_b2cSignup";
        public static string SignInPolicyId = "b2c_1_b2csignin";
        public static string ChangePasswordPolicyId = "B2C_1_b2cSSPR";
        public static string signInMFAPolicyId = "B2C_1_Signin_MFA";
        public static string UserClaimsURL = "http://schemas.microsoft.com/identity/claims/objectidentifier";

    }

    public class B2CConfig
    {
        public static string AuthorizeURL = "https://login.microsoftonline.com/{0}/oauth2/v2.0/authorize?p={1}&client_id={2}&response_type=code&redirect_uri={3}&response_mode=query&scope=openid&state=arbitrary_data_you_can_receive_in_the_response";
        public static string TokenURL = "https://login.microsoftonline.com/{0}/oauth2/v2.0/token?p={1}&grant_type={2}&client_secret={3}&client_id={4}&code={5}";
        //public static string AuthorizeURLIOS = "https://login.microsoftonline.com/{0}/oauth2/v2.0/authorize?p={1}&client_id={2}&response_type=code&redirect_uri=com.onmicrosoft.csu://iosresponse/&response_mode=query&scope=openid&state=arbitrary_data_you_can_receive_in_the_response";
        public static string TokenURLIOS = "https://login.microsoftonline.com/{0}/oauth2/v2.0/token?p={1}&grant_type={2}&client_id={3}&code={4}";
        public static string ChangePasswordURL = "https://login.microsoftonline.com/{0}/oauth2/v2.0/authorize?p={1}&client_Id={2}&nonce=defaultNonce&redirect_uri={3}&scope=openid&response_type=id_token&prompt=login";
        //https://login.microsoftonline.com/CSUB2C.onmicrosoft.com/oauth2/v2.0/authorize?p=B2C_1_b2cSSPR&client_Id=3bdf8223-746c-42a2-ba5e-0322bfd9ff76&nonce=defaultNonce&redirect_uri=com.onmicrosoft.csu://iosresponse/&scope=openid&response_type=id_token&prompt=login
        public static string ClientId = "3bdf8223-746c-42a2-ba5e-0322bfd9ff76";
        public static string ClientSecret = ":63s]7`4F^3261*}";
        public static string Tenant = "csub2c.onmicrosoft.com";
        public static string Grant_type = "authorization_code";
        public static string Token_Grant_type = "offline_access";
        public static string Redirect_Uri = "http://localhost:65328";

    }
}
