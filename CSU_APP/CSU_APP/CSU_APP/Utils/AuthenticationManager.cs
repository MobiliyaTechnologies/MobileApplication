using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xamarin.Forms;
using System.Collections.Generic;
using CSU_APP.Models;

namespace CSU_APP
{
    class AuthenticationManager
    {
        private static AuthenticationManager instance = null;
        private static readonly object padlock = new object();
        public AccessTokenResponse localToken;
        public Models.UserDetails userData;

        AuthenticationManager()
        {
        }

        public static AuthenticationManager Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new AuthenticationManager();
                    }
                    return instance;
                }
            }
        }

        public class ResetPasswordResponse
        {
            public int Status_Code;
            public string Message;
            public string First_Name;
            public string Last_Name;
            public string Email;
            public int User_Id;
        }

        public class LogoutResponse
        {
            public int Status_Code;
            public string Message;
        }
        public class Token
        {
            public string AccessToken;
            public string RefreshToken;
        }
        public class AccessTokenResponse
        {
            public Token tokens;
        }



        public async Task<Models.UserDetails> performInternalLogin(string u, string p)
        {
            Contract.Ensures(Contract.Result<Task>() != null);
            try
            {
                if (u == null || u == "" || p == null || p == "")
                {
                    return null;
                }

                HttpClient client = new HttpClient();
                string uri = Utils.Constants.SERVER_BASE_URL + Utils.Constants.API_SIGN_IN;

                string sContentType = "application/json";
                string postData = "{\"Email\":\"" + u + "\",\"Password\":\"" + p + "\"}";
                var oTaskPostAsync = client.PostAsync(uri, new StringContent(postData, Encoding.UTF8, sContentType));

                HttpResponseMessage response = await oTaskPostAsync;
                string responseStr = await response.Content.ReadAsStringAsync();

                Models.UserDetails loginResponce = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.UserDetails>(responseStr);
                Debug.WriteLine("###\n" + responseStr + "\n###" + loginResponce.First_Name);
                this.userData = loginResponce;

                if (loginResponce.Status_Code == 200)
                {
                    Debug.WriteLine("###\n" + responseStr + "\n###" + loginResponce.First_Name);
                    return loginResponce;
                }
                else
                {
                    //MessagingCenter.Send<Page>(page,"Error while login");
                    return null;
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
        }

        public async Task<bool> performInternalLogOut(string email)
        {
            Contract.Ensures(Contract.Result<Task>() != null);
            try
            {
                HttpClient client = new HttpClient();
                string uri = Utils.Constants.SERVER_BASE_URL + Utils.Constants.API_SIGN_OUT;

                string sContentType = "application/json";
                string postData = "{\"Email\":\"" + email + "\"}";
                var oTaskPostAsync = client.PostAsync(uri, new StringContent(postData, Encoding.UTF8, sContentType));

                HttpResponseMessage response = await oTaskPostAsync;
                string responseStr = await response.Content.ReadAsStringAsync();
                LogoutResponse loginResponce = Newtonsoft.Json.JsonConvert.DeserializeObject<LogoutResponse>(responseStr);
                if (loginResponce.Status_Code == 200)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }
        }

        public AccessTokenResponse getAccessTokenContent()
        {
            //Sync call
            var httpClient = new HttpClient();
            HttpResponseMessage response = httpClient.GetAsync("http://csuwebapp.azurewebsites.net/PowerBIService.asmx/GetAccessToken").Result;

            if (response.IsSuccessStatusCode)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                AccessTokenResponse tokenResp = JsonConvert.DeserializeObject<AccessTokenResponse>(data);
                Debug.WriteLine(data);
                this.localToken = tokenResp;
                return tokenResp;
            }
            else
            {
                Debug.WriteLine(response);
                return null;
            }
        }

        public string getBIReportContent()
        {
            Contract.Ensures(Contract.Result<Task>() != null);

            HttpClientHandler handler = new HttpClientHandler();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, new Uri("https://app.powerbi.com/embed?dashboardId=cea6812f-9d03-4394-ae7b-cbdb779d9b6f&tileId=da251716-8cef-47e3-a2b7-1aa1e7e8bb9d&width=500&height=300", UriKind.Absolute));
            request.Headers.TransferEncodingChunked = true;
            using (HttpClient httpClient = new HttpClient(handler))
            {
                httpClient.DefaultRequestHeaders.Clear();

                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("utf-8"));

                request.Headers.Clear();
                string token = this.localToken.tokens.AccessToken;
                request.Headers.Add("Authorization", "Bearer " + token);


                foreach (var header in httpClient.DefaultRequestHeaders)
                {
                    var sb = new StringBuilder();
                    foreach (var single in header.Value)
                    {
                        sb.Append(single + " ");
                    }
                    System.Diagnostics.Debug.WriteLine("Header - {0} {1}", header.Key, sb.ToString());
                }

                foreach (var header in request.Headers)
                {
                    var sb = new StringBuilder();
                    foreach (var single in header.Value)
                    {
                        sb.Append(single + " ");
                    }
                    System.Diagnostics.Debug.WriteLine("Header - {0} {1}", header.Key, sb.ToString());
                }

                HttpResponseMessage responseObj = httpClient.SendAsync(request).Result;
                //HttpResponseMessage response = httpClient.GetAsync("http://csuwebapp.azurewebsites.net/PowerBIService.asmx/GetAccessToken").Result;
                responseObj.EnsureSuccessStatusCode();
                //Task<string> contentsTask;
                if (responseObj.IsSuccessStatusCode)
                {
                    var o = responseObj.Content.ReadAsStringAsync().Result;
                    //string jsonObj = JsonConvert.DeserializeObject<string>(o);
                    return o;
                }
                else
                {
                    return null;
                }

                //string contents = await contentsTask;
                //return contents;

            }

        }

        public async Task<bool> changePassword(string email, string oldPW, string newPW)
        {
            if (email == null || email == "" || oldPW == null || oldPW == "" || newPW == null || newPW == "")
            {
                return false;
            }

            HttpClient client = new HttpClient();
            //string uri = "http://powergridrestservice.azurewebsites.net/api/changepassword";
            string uri = Utils.Constants.SERVER_BASE_URL + Utils.Constants.API_CHANGE_PASSWORD;

            string sContentType = "application/json";

            string postData = "{\"Email\":\"" + email + "\",\"Password\":\"" + oldPW + "\",\"New_Password\":\"" + newPW + "\"}";
            var oTaskPostAsync = client.PostAsync(uri, new StringContent(postData, Encoding.UTF8, sContentType));

            HttpResponseMessage response = await oTaskPostAsync;
            string responseStr = await response.Content.ReadAsStringAsync();

            ResetPasswordResponse resp = JsonConvert.DeserializeObject<ResetPasswordResponse>(responseStr);
            Debug.WriteLine("###\n" + responseStr + "\n###" + resp.First_Name);


            if (resp.Status_Code == 200)
            {
                Debug.WriteLine("###\n" + responseStr + "\n###" + resp.First_Name);
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<ForgotPasswordModel> forgotPassword(string email)
        {
            if (email == null || email == "")
            {
                return null;
            }

            HttpClient client = new HttpClient();
            string uri = Utils.Constants.SERVER_BASE_URL + Utils.Constants.API_FORGOT_PASSWORD;

            string sContentType = "application/json";

            string postData = "{\"Email\":\"" + this.userData.Email + "\"}";
            var oTaskPostAsync = client.PostAsync(uri, new StringContent(postData, Encoding.UTF8, sContentType));

            HttpResponseMessage response = await oTaskPostAsync;
            string responseStr = await response.Content.ReadAsStringAsync();

            ForgotPasswordModel resp = JsonConvert.DeserializeObject<ForgotPasswordModel>(responseStr);
            Debug.WriteLine("Forgot Password Response\n" + responseStr);


            if (resp.Status_Code == 200)
            {
                Debug.WriteLine("Forgot Password Response\n" + responseStr + "\n###" + resp.Message);
                return resp;
            }
            else
            {
                return null;
            }
        }

        public async Task<IList<Models.MeterDetails>> getMeterDetails(int userId)
        {
            Contract.Ensures(Contract.Result<Task>() != null);
            IList<Models.MeterDetails> meterList = null;
            try
            {
                HttpClient client = new HttpClient();
                string uri = Utils.Constants.SERVER_BASE_URL + 
                    Utils.Constants.API_GET_METER_LIST + "/" + userId;
               
                var oTaskGetAsync = client.GetAsync(uri);

                HttpResponseMessage response = await oTaskGetAsync;
                string responseStr = await response.Content.ReadAsStringAsync();
                if (responseStr != null) { 

                    JArray array = JArray.Parse(responseStr);
                    meterList = array.ToObject<IList<Models.MeterDetails>>();
                }
                return meterList;

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
        }

        public async Task<IList<Models.MonthlyConsumptionDetails>> getMonthlyConsumptionDetails(int userId)
        {
            Contract.Ensures(Contract.Result<Task>() != null);
            IList<Models.MonthlyConsumptionDetails> monthlyList = null;
            try
            {
                HttpClient client = new HttpClient();
                string uri = Utils.Constants.SERVER_BASE_URL +
                    Utils.Constants.API_GET_MONTHLY_CONSUMPTION + "/" + userId;

                var oTaskGetAsync = client.GetAsync(uri);

                HttpResponseMessage response = await oTaskGetAsync;
                string responseStr = await response.Content.ReadAsStringAsync();
                if (responseStr != null)
                {

                    JArray array = JArray.Parse(responseStr);
                    monthlyList = array.ToObject<IList<Models.MonthlyConsumptionDetails>>();
                }
                return monthlyList;

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
        }

        public async Task<Models.MeterReports> getMeterReports(int userId, string meterSerial)
        {
            Contract.Ensures(Contract.Result<Task>() != null);
            Models.MeterReports meterReports = null;
            try
            {
                HttpClient client = new HttpClient();
                string uri = Utils.Constants.SERVER_BASE_URL +
                    Utils.Constants.API_GET_METER_REPORTS + "/" + userId + "/" + meterSerial;

                var oTaskGetAsync = client.GetAsync(uri);

                HttpResponseMessage response = await oTaskGetAsync;
                string responseStr = await response.Content.ReadAsStringAsync();
                if (responseStr != null)
                {
                    meterReports = JsonConvert.DeserializeObject<Models.MeterReports>(responseStr);
                    Debug.WriteLine(meterReports);

                }
                return meterReports;

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
        }
    }
}
