using System.Collections.Generic;
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
        public void should_be_case_insensitive()
        {
            var builder = new MockedHttpServerBuilder();
            builder
                .WhenGet("/Test")
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
        public void should_read_as_model_wen_media_type_is_json()
        {
            var request = default(List<StreamEntity>).ToRequest();
            var builder = new MockedHttpServerBuilder();
            builder
                .WhenPost("/streams/test")
                .WithRequest<List<StreamEntity>>(r => request = r)
                .Respond(HttpStatusCode.OK);
            using (builder.Build("http://localhost:1122"))
            {
                using (var httpClient = new HttpClient())
                {
                    const string result = @"[
                      {
                        ""eventId"": ""e1fdf1f0-a66d-4f42-95e6-d6588cc22e9b"",
                        ""id"": 0
                      }
                    ]";

                    var content = new StringContent(result);
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    var response = httpClient.PostAsync("http://localhost:1122/streams/test", content).Result;
                    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                    Assert.NotNull(request.RequestBody);
                }
            }
        }

        [Fact]
        public void should_read_string_as_request_body_for_unknow_content_type()
        {
            var request = default(object).ToRequest();
            var builder = new MockedHttpServerBuilder();
            builder
                .WhenPost("/streams/test")
                .WithRequest<object>(r => request = r)
                .Respond(HttpStatusCode.OK);
            using (builder.Build("http://localhost:1122"))
            {
                using (var httpClient = new HttpClient())
                {
                    const string result = @"[
                      {
                        ""eventId"": ""e1fdf1f0-a66d-4f42-95e6-d6588cc22e9b"",
                        ""id"": 0
                      }
                    ]";
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
            builder.WhenGet("/test")
                .RespondContent(HttpStatusCode.OK, r => new StringContent(result));
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
        public void should_be_able_to_accept_http_content_multiple_times()
        {
            var builder = new MockedHttpServerBuilder();
            const string result = " a \"c\" b ";
            builder.WhenGet("/test")
                .RespondContent(HttpStatusCode.OK, request => new StringContent(result));
            using (builder.Build("http://localhost:1122"))
            {
                using (var httpClient = new HttpClient())
                {
                    Assert.Equal(result,
                        httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, "http://localhost:1122/test")).Result.Content.ReadAsStringAsync().Result); // raw string

                    Assert.Equal(result,
                        httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, "http://localhost:1122/test")).Result.Content.ReadAsStringAsync().Result); // raw string
                }
            }

        }

        [Fact]
        public void should_be_able_to_accept_raw_object()
        {
            var builder = new MockedHttpServerBuilder();
            int result = 56;
            builder.WhenGet("/test")
                .Respond(HttpStatusCode.OK, result);
            using (builder.Build("http://localhost:1122"))
            {
                using (var httpClient = new HttpClient())
                {
                    var response = httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, "http://localhost:1122/test")).Result;
                    var actual = response.Content.ReadAsStringAsync().Result;
                    Assert.Equal(result.ToString(), actual);
                }
            }
        }

        [Fact]
        public void should_be_able_to_reconfigure_server_on_the_fly()
        {
            var builder = new MockedHttpServerBuilder();
            const string content = " a \"c\" b ";
            builder.WhenGet("/test")
                .RespondContent(HttpStatusCode.OK, request => new StringContent(content));

            using (var server = builder.Build("http://localhost:1122"))
            using (var httpClient = new HttpClient())
            {
                var response = httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, "http://localhost:1122/test")).Result;
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal(content, response.Content.ReadAsStringAsync().Result);

                var newBuilder = new MockedHttpServerBuilder();
                newBuilder.WhenGet("/test").Respond(HttpStatusCode.BadRequest);

                newBuilder.Reconfigure(server, true);

                Assert.Equal(HttpStatusCode.BadRequest, httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, "http://localhost:1122/test")).Result.StatusCode);
            }
        }

        [Fact]
        public void should_be_able_to_reconfigure_server_on_the_fly_with_preserving_existing_mocks()
        {
            var builder = new MockedHttpServerBuilder();
            builder.WhenGet("/test").Respond(HttpStatusCode.BadRequest);
            builder.WhenPut("/test").Respond(HttpStatusCode.Accepted);

            using (var server = builder.Build("http://localhost:1122"))
            using (var httpClient = new HttpClient())
            {
                Assert.Equal(HttpStatusCode.BadRequest, httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, "http://localhost:1122/test")).Result.StatusCode);
                Assert.Equal(HttpStatusCode.Accepted, httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Put, "http://localhost:1122/test")).Result.StatusCode);

                var newBuilder = new MockedHttpServerBuilder();
                newBuilder.WhenGet("/test").Respond(HttpStatusCode.OK);
                newBuilder.Reconfigure(server, false);

                Assert.Equal(HttpStatusCode.OK, httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, "http://localhost:1122/test")).Result.StatusCode);
                Assert.Equal(HttpStatusCode.Accepted, httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Put, "http://localhost:1122/test")).Result.StatusCode);
            }
        }

        [Fact]
        public void should_be_able_to_reconfigure_server_on_the_fly_with_removing_existing_mocks()
        {
            var builder = new MockedHttpServerBuilder();
            builder.WhenGet("/test").Respond(HttpStatusCode.BadRequest);
            builder.WhenPut("/test").Respond(HttpStatusCode.Accepted);

            using (var server = builder.Build("http://localhost:1122"))
            using (var httpClient = new HttpClient())
            {
                Assert.Equal(HttpStatusCode.BadRequest, httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, "http://localhost:1122/test")).Result.StatusCode);
                Assert.Equal(HttpStatusCode.Accepted, httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Put, "http://localhost:1122/test")).Result.StatusCode);

                var newBuilder = new MockedHttpServerBuilder();
                newBuilder.WhenGet("/test").Respond(HttpStatusCode.InternalServerError);
                newBuilder.Reconfigure(server, true);

                //altered behavior
                Assert.Equal(HttpStatusCode.InternalServerError, httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, "http://localhost:1122/test")).Result.StatusCode);
                //the default response
                Assert.Equal(HttpStatusCode.NotFound, httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Put, "http://localhost:1122/test")).Result.StatusCode);
            }
        }

        [Fact]
        public void should_be_able_to_valid_request()
        {
            var builder = new MockedHttpServerBuilder();
            const string result = " a \"c\" b ";

            var actualRequest = ActualRequest.ToRequest();

            builder.WhenPost("/test")
                .WithRequest(r => actualRequest = r)
                .RespondContent(HttpStatusCode.OK, r => new StringContent(result));

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
                    Assert.Equal("a", (string)actualRequest.RequestBody.Field);
                    Assert.Equal("b", (string)actualRequest.RequestBody.Field2);
                }
            }

        }

        [Fact]
        public void should_be_able_to_accept_custom_header()
        {
            var builder = new MockedHttpServerBuilder();
            const string content = "dummy";
            const string headerValue = "testHeaderValue";
            builder.WhenGet("/test")
                .RespondContent(HttpStatusCode.OK, r => new StringContent(content))
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
        [Fact]
        public void should_be_able_to_match_dollor()
        {
            var builder = new MockedHttpServerBuilder();
            builder.WhenGet("/te$st").Respond(HttpStatusCode.OK);
            using (builder.Build("http://localhost:1122"))
            {
                using (var httpClient = new HttpClient())
                {
                    var response = httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, "http://localhost:1122/te$st")).Result;
                    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                }
            }

        }

        [Fact]
        public void should_be_able_create_server_and_build()
        {
            using (var server = new MockedHttpServer("http://localhost:1122"))
            {
                var builder = new MockedHttpServerBuilder();
                builder.WhenGet("/test").Respond(HttpStatusCode.OK);
                builder.Build(server);
                using (var httpClient = new HttpClient())
                {
                    var response = httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, "http://localhost:1122/test")).Result;
                    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                }

            }
        }

        [Fact]
        public void should_be_able_to_retrive_request()
        {
            var builder = new MockedHttpServerBuilder();
            var requestRetriever = builder.WhenGet("/test1").Respond(HttpStatusCode.OK).Retrieve();
            using (builder.Build("http://localhost:1122"))
            {
                using (var httpClient = new HttpClient())
                {
                    Assert.Null(requestRetriever());
                    var response = httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, "http://localhost:1122/test0")).Result;
                    Assert.Null(requestRetriever());
                    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

                    response = httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, "http://localhost:1122/test1")).Result;
                    var actualRequest = requestRetriever();
                    Assert.NotNull(actualRequest);
                    Assert.Equal(HttpStatusCode.OK, response.StatusCode);                    
                    Assert.Equal("GET", actualRequest.Method);
                    Assert.Equal("http://localhost:1122/test1", actualRequest.RequestUri.ToString());

                    response = httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, "http://localhost:1122/test2")).Result;
                    var retriever = requestRetriever();
                    Assert.NotNull(retriever);
                    Assert.Equal("http://localhost:1122/test1", retriever.RequestUri.ToString());
                    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

                }
            }
        }

        [Fact]
        public void should_be_able_to_match_and_retrive_request()
        {
            var builder = new MockedHttpServerBuilder();
            var requestRetriever = builder.WhenGet("/te$st").Respond(HttpStatusCode.OK)
                .MatchRequest(r => true)
                .Retrieve();
            using (builder.Build("http://localhost:1122"))
            {
                using (var httpClient = new HttpClient())
                {
                    var actualRequest = requestRetriever();
                    Assert.Null(actualRequest);
                    var response = httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, "http://localhost:1122/te$st")).Result;
                    actualRequest = requestRetriever();
                    Assert.NotNull(actualRequest);
                }
            }
        }

        [Fact]
        public void should_be_able_to_process_string_as_json()
        {
            var builder = new MockedHttpServerBuilder();
            var retrieve = builder.WhenPut("/te$st").Respond(HttpStatusCode.OK, new {}).Retrieve();
            using (builder.Build("http://localhost:1122"))
            {
                using (var httpClient = new HttpClient())
                {
                    var response = httpClient.PutAsJsonAsync("http://localhost:1122/te$st", "abc").Result;
                    Assert.Equal(HttpStatusCode.OK,response.StatusCode);
                    Assert.Equal("abc", retrieve().RequestBody);
                }
            }
        }
    }
}
