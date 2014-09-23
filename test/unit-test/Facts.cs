using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
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
            builder.WhenGet(string.Format("test"))
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

    }
}
