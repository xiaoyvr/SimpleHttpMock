using System.Linq;
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
        public void should_read_string_as_request_body_for_unknow_content_type()
        {
            var request = default(object).ToRequest();
            var builder = new MockedHttpServerBuilder();
            builder
                .WhenPost(string.Format("/streams/test"))
                .WithRequest<object>(r => request = r)
                .Respond(HttpStatusCode.OK);
            using (builder.Build("http://localhost:1122"))
            {
                using (var httpClient = new HttpClient())
                {
                    const string result = @"[{'eventId': 'e1fdf1f0-a66d-4f42-95e6-d6588cc22e9b','id': 0}}]";
                    var content = new StringContent(result);
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.eventstore.events+json");

                    var response = httpClient.PostAsync("http://localhost:1122/streams/test", content).Result;
                    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                    Assert.NotNull(request.RequestBody);
                }
            }
        }

        [Fact]
        public void should_matches_url_when_it_is_absolute_uri()
        {
            var builder = new MockedHttpServerBuilder();
            builder
                .WhenGet("http://localhost:1122/test")
                .Respond(HttpStatusCode.InternalServerError);
            using (builder.Build("http://localhost:1122"))
            {
                using (var httpClient = new HttpClient())
                {
                    var response = httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, "http://localhost:1122/test")).Result;
                    Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
                }
            }
        }


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

        [Fact]
        public void should_be_able_to_accept_custom_header()
        {
            var builder = new MockedHttpServerBuilder();
            const string content = "dummy";
            const string headerValue = "testHeaderValue";
            builder.WhenGet(string.Format("/test"))
                .RespondContent(HttpStatusCode.OK, new StringContent(content))
                .RespondHeaders(new { headerKey = headerValue });
            using (builder.Build("http://localhost:1122"))
            {
                using (var httpClient = new HttpClient())
                {
                    var response = httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, "http://localhost:1122/test")).Result;
                    Assert.Equal(content, response.Content.ReadAsStringAsync().Result);
                    Assert.Equal(headerValue, response.Headers.GetValues("headerKey").First());
                }
            }

        }

    }
}
