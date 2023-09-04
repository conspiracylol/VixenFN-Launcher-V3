using Newtonsoft.Json.Linq;
using RestSharp;
using System.Diagnostics;
using System.Threading;

namespace VxnAuth
{
    public class Class1
    {
        public class AccountInfo
        {
            public static string DisplayName { get; set; }
            public static string AccountId { get; set; }
        }

        public class Auth
        {
            private static RestClient restClient;

            static Auth()
            {
                restClient = new RestClient("https://account-public-service-prod03.ol.epicgames.com/account/api/oauth");
            }

            public static string GetDevicecodetoken()
            {
                RestRequest restRequest = CreateRestRequest(Method.Post, "token");
                restRequest.AddHeader("Authorization", "Basic OThmN2U0MmMyZTNhNGY4NmE3NGViNDNmYmI0MWVkMzk6MGEyNDQ5YTItMDAxYS00NTFlLWFmZWMtM2U4MTI5MDFjNGQ3");
                restRequest.AddParameter("grant_type", "client_credentials");

                try
                {
                    var response = restClient.Execute(restRequest);

                    return ParseToken(response.Content);
                }
                catch
                {
                    Process.GetCurrentProcess().Kill();
                    return "error";
                }
            }

            public static string GetDeviceCode(string authToken)
            {
                RestRequest restRequest = CreateRestRequest(Method.Post, "deviceAuthorization");
                restRequest.AddHeader("Authorization", "bearer " + authToken);
                restRequest.AddParameter("Content-Type", "application/x-www-form-urlencoded");

                var response = restClient.Execute(restRequest);
                var json = JObject.Parse(response.Content);
                string deviceCode = json.Value<string>("device_code");
                string verificationUriComplete = json.Value<string>("verification_uri_complete");

                Process.Start(verificationUriComplete);

                string accessToken = WaitForAccessToken(deviceCode);

                return accessToken;
            }

            public static string GetExchange(string token)
            {
                RestRequest restRequest = CreateRestRequest(Method.Get, "exchange");
                restRequest.AddHeader("Authorization", "bearer " + token);

                var response = restClient.Execute(restRequest);

                return ParseExchange(response.Content);
            }

            private static RestRequest CreateRestRequest(Method method, string resource)
            {
                RestRequest restRequest = new RestRequest(resource);
                restRequest.Method = method;

                return restRequest;
            }

            private static string ParseToken(string content)
            {
                var token = JObject.Parse(content);

                return token.Value<string>("access_token");
            }

            private static string ParseExchange(string content)
            {
                var exchange = JObject.Parse(content);

                return exchange.Value<string>("code");
            }

            private static string WaitForAccessToken(string deviceCode)
            {
                while (true)
                {
                    RestRequest restRequest = CreateRestRequest(Method.Post, "token");
                    restRequest.AddHeader("Authorization", "basic OThmN2U0MmMyZTNhNGY4NmE3NGViNDNmYmI0MWVkMzk6MGEyNDQ5YTItMDAxYS00NTFlLWFmZWMtM2U4MTI5MDFjNGQ3");
                    restRequest.AddParameter("grant_type", "device_code");
                    restRequest.AddParameter("device_code", deviceCode);

                    var response = restClient.Execute(restRequest);
                    var json = JObject.Parse(response.Content);

                    if (json.ContainsKey("error"))
                    {
                        Thread.Sleep(200);

                    }
                    else
                    {
                        AccountInfo.AccountId = json.Value<string>("account_id");
                        AccountInfo.DisplayName = json.Value<string>("displayName");

                        return json.Value<string>("access_token");
                    }
                }
            }
        }
    }
}