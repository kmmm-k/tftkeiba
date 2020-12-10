using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;
using System.Net;
using Newtonsoft.Json;

namespace tftkeiba.Utils
{
    public class CallAPI
    {
        private static readonly HttpClient _httpClient;

        static CallAPI()
        {
            _httpClient = new HttpClient();
        }

        public static async Task<T> SendRiotAsync<T>(string endpoint, HttpMethod method, Dictionary<string,string> content = null)
            where T : class, new()
        {
            Dictionary<string, string> headers = new Dictionary<string, string>() { { "X-Riot-Token", Properties.Settings.Default.APIKey } };
            return await SendAsync<T>(endpoint, method, headers, content);
        }

        /// <summary>
        /// Uriへリクエストを送信しレスポンス(JSON)をT型のオブジェクトに格納して返す
        /// </summary>
        /// <typeparam name="T">Response Type</typeparam>
        /// <param name="endpoint">例："https://americas.api.riotgames.com/riot/account/v1/accounts/by-puuid/test"</param>
        /// <param name="method">HttpMethod.Post, Get ...</param>
        /// <param name="content">POSTの際にBodyに格納するコンテンツ</param>
        /// <returns></returns>
        public static async Task<T> SendAsync<T>(string endpoint, HttpMethod method, Dictionary<string, string> headers = null, Dictionary<string,string> content = null)
            where T : class, new()
        {
            var ret = new T();
            var httpRequestMessage = new HttpRequestMessage
            {
                Method = method,
                RequestUri = new Uri(endpoint),
                Headers = {
                //{ HttpRequestHeader.Authorization.ToString(), "Bearer xxxxxxxxxxxxxxxxxxxx" },
                { HttpRequestHeader.Accept.ToString(), "application/json" }
            }};

            if (headers != null)
            {
                foreach (var key in headers.Keys)
                {
                    httpRequestMessage.Headers.Add(key, headers[key]);
                }
            }

            if (content != null)
            {
                var json = JsonConvert.SerializeObject(content);
                httpRequestMessage.Content = new StringContent(json, Encoding.UTF8, "application/json");
            }

            var res = await _httpClient.SendAsync(httpRequestMessage);
            if (res.IsSuccessStatusCode == false)
            {
                throw new Exception(string.Format("{0}:{1}", Enum.Format(typeof(HttpStatusCode), res.StatusCode, "d"), res.ReasonPhrase));
            }
            var responseContent = await res.Content.ReadAsStringAsync();

            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(responseContent)))
            {
                var ser = new DataContractJsonSerializer(ret.GetType());
                ret = ser.ReadObject(ms) as T;
            }

            return ret;
        }
    }
}
