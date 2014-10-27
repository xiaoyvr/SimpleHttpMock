using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using SimpleHttpMock;
using Xunit;

namespace test
{
    public class Facts
    {
        [Fact]
        public void should_be_able_to_accept_string_content()
        {
            var builder = new MockedHttpServerBuilder();
            const string result = " a \"c\" b ";
            builder.WhenGet(string.Format("/test"))
                .RespondContent(HttpStatusCode.OK, new StringContent(result));
            using (builder.Build("http://localhost:1122"))
            {
                using (var httpClient = new HttpClient())
                {
                    var response = httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, "http://localhost:1122/test")).Result;
                    Assert.Equal(result, response.Content.ReadAsStringAsync().Result); // raw string
                }
            }

        }

        [Fact]
        public void should_be_able_to_valid_request()
        {
            var builder = new MockedHttpServerBuilder();
            const string result = " a \"c\" b ";

            var actualRequest = ActualRequest.ToRequest();

            builder.WhenPost(string.Format("/test"))
                .WithRequest(r => actualRequest = r)
                .RespondContent(HttpStatusCode.OK, new StringContent(result));

            using (builder.Build("http://localhost:1122"))
            {
                using (var httpClient = new HttpClient())
                {
                    var requestBody = new { Field = "a", Field2 = "b" };
                    var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:1122/test")
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(requestBody))
                    };

                    request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    var response = httpClient.SendAsync(request).Result;
                    Assert.Equal(result, response.Content.ReadAsStringAsync().Result); // raw string
                    Assert.Equal("a", actualRequest.RequestBody.Field);
                    Assert.Equal("b", actualRequest.RequestBody.Field2);
                }
            }

        }

    }
}
