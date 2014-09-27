using System.Net.Http;

namespace test
{
    public class TestBase
    {
        protected static HttpResponseMessage Get(string requestUr)
        {
            return SendHttpRequest(requestUr, HttpMethod.Get);
        }

        protected static HttpResponseMessage SendHttpRequest(string requestUri, HttpMethod httpMethod)
        {
            using (var httpClient = new HttpClient())
            {
                var httpRequestMessage = new HttpRequestMessage(httpMethod, requestUri);
                return httpClient.SendAsync(httpRequestMessage).Result;
            }
        }
    }
}