using System.Net.Http;
using Newtonsoft.Json;

namespace test
{
    public class TestBase
    {
        protected static HttpResponseMessage Get(string requestUr)
        {
            return SendHttpRequest(requestUr, HttpMethod.Get);
        }

        private static StringContent BuildRequestContent(object data)
        {
            return data == null ? null:  new StringContent(JsonConvert.SerializeObject(data));
        }

        protected HttpResponseMessage Post(string requestUri, object data)
        {
            return SendHttpRequest(requestUri, HttpMethod.Post, data);
        }

        protected static HttpResponseMessage SendHttpRequest(string requestUri, HttpMethod httpMethod, object data = null)
        {
            using (var httpClient = new HttpClient())
            {
                var httpRequestMessage = new HttpRequestMessage(httpMethod, requestUri)
                    {
                        Content = BuildRequestContent(data)
                    };
                return httpClient.SendAsync(httpRequestMessage).Result;
            }
        }
    }
}