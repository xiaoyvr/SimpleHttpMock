using System.Net;
using System.Net.Http;
using SimpleHttpMock;
using Xunit;

namespace test
{
    public class UriMatchFacts
    {
        [Fact]
        public void should_match_url_contains_question_mark()
        {
            var serverBuilder = new MockedHttpServerBuilder();
            serverBuilder
                .WhenGet("/staff?employeeId=Staff0001")
                .Respond(HttpStatusCode.InternalServerError);
            const string baseAddress = "http://localhost:1122";
            using (serverBuilder.Build(baseAddress))
            {
                using (var httpClient = new HttpClient())
                {
                    const string requestUri = "http://localhost:1122/staff?employeeId=Staff0001";
                    var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri);
                    var response = httpClient.SendAsync(httpRequestMessage).Result;
                    Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
                }
            }
        }
    }
}