using System.Net;
using SimpleHttpMock;
using Xunit;

namespace test
{
    public class UriMatchFacts : TestBase
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
                const string requestUri = "http://localhost:1122/staff?employeeId=Staff0001";
                var response = Get(requestUri);
                Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
            }
        }
    }
}