using System.Net;
using SimpleHttpMock;
using Xunit;

namespace test
{
    public class HarmcrestGrammarFacts : TestBase
    {
        private const string BaseAddress = "http://localhost:1122";
        private const string RequestUri = "http://localhost:1122/staff?employeeId=Staff0001";

        [Fact]
        public void should_support_is_regexp()
        {
            var serverBuilder = new MockedHttpServerBuilder();
            serverBuilder
                .WhenGet(It.IsRegex(@"/staff\?employeeId=Staff0001"))
                .Respond(HttpStatusCode.InternalServerError);

            using (serverBuilder.Build(BaseAddress))
            {
                var response = Get(RequestUri);
                Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
            }
        }

        [Fact]
        public void should_support_it_is_star_wildcard()
        {
            var serverBuilder = new MockedHttpServerBuilder();
            serverBuilder
                .WhenGet(It.IsWildcard(@"/staff*"))
                .Respond(HttpStatusCode.InternalServerError);

            using (serverBuilder.Build(BaseAddress))
            {
                var response = Get(RequestUri);
                Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
            }
        }

        [Fact]
        public void should_support_it_is_question_mark_wildcard()
        {
            var serverBuilder = new MockedHttpServerBuilder();
            serverBuilder
                .WhenGet(It.IsWildcard(@"/staffs/?"))
                .Respond(HttpStatusCode.InternalServerError);

            using (serverBuilder.Build(BaseAddress))
            {
                var response = Get(string.Format("{0}/staffs/1", BaseAddress));
                Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
            }
        }

        [Fact]
        public void should_support_it_is()
        {
            var serverBuilder = new MockedHttpServerBuilder();
            serverBuilder
                .WhenGet(It.Is("/staff?employeeId=Staff0001"))
                .Respond(HttpStatusCode.InternalServerError);

            using (serverBuilder.Build(BaseAddress))
            {
                var response = Get(RequestUri);
                Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
            }

        }
    }
}