using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;

namespace CSU_PORTABLE.Utils
{
    public class InvokeApi
    {
        private static string ServerBaseURL = null;

        public static void SetDomainUrl(string domainUrl)
        {
            ServerBaseURL = domainUrl + "/api/";
        }

        public static async Task<HttpResponseMessage> Invoke(string serviceUrl, string json, HttpMethod method, string token = "", DemoStage dateFilter = DemoStage.None)
        {
            HttpClient client = new HttpClient();
            try
            {
                string requestURL = ServerBaseURL + serviceUrl;
                if (dateFilter != DemoStage.None)
                {
                    requestURL = requestURL + "?DateFilter=" + (int)dateFilter;
                }
                HttpRequestMessage request = new HttpRequestMessage(method, requestURL);
                client.DefaultRequestHeaders
                         .Accept
                         .Add(new MediaTypeWithQualityHeaderValue("application/json"));//ACCEPT header

                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
                if (!string.IsNullOrEmpty(json))
                {
                    request.Content = new StringContent(json, Encoding.UTF8, "application/json");//CONTENT-TYPE header;
                                                                                                 //request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bootstrapContext.Token);
                }
                return await client.SendAsync(request);
            }
            catch (Exception ex)
            {
                HttpResponseMessage responseMessage = new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    ReasonPhrase = ex.Message,
                    Content = new StringContent(ex.Message)
                };

                return responseMessage;
            }

        }


        public static async Task<HttpResponseMessage> Authenticate(string serviceUrl, string json, HttpMethod method)
        {
            HttpClient client = new HttpClient();
            try
            {
                HttpRequestMessage request = new HttpRequestMessage(method, serviceUrl);
                client.DefaultRequestHeaders
                         .Accept
                         .Add(new MediaTypeWithQualityHeaderValue("application/json"));//ACCEPT header
                if (!string.IsNullOrEmpty(json))
                {
                    request.Content = new StringContent(json, Encoding.UTF8, "application/json");//CONTENT-TYPE header;
                                                                                                 //request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bootstrapContext.Token);
                }
                return await client.SendAsync(request);
            }
            catch (Exception ex)
            {
                HttpResponseMessage responseMessage = new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    ReasonPhrase = ex.Message,
                    Content = new StringContent(ex.Message)
                };

                return responseMessage;
            }


        }

    }
}
