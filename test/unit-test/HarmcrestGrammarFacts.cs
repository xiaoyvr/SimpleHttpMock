using System.Net;
using System.Net.Http;
using SimpleHttpMock;
using Xunit;
using Xunit.Extensions;

namespace test
{
    public class HarmcrestGrammarFacts : TestBase
    {
        private const string BaseAddress = "http://localhost:1122";
        private const string RequestUri = "http://localhost:1122/staff?employeeId=Staff0001";

        [Theory]
        [InlineData("/staffs", HttpStatusCode.InternalServerError)]
        [InlineData("/users", HttpStatusCode.InternalServerError)]
        [InlineData("/assignees", HttpStatusCode.NotFound)]
        public void should_support_is_regex(string url, HttpStatusCode expectedStatusCode)
        {
            var serverBuilder = new MockedHttpServerBuilder();
            serverBuilder
                .WhenGet(Matchers.Regex(@"/(staff)|(user)s"))
                .Respond(HttpStatusCode.InternalServerError);

            using (serverBuilder.Build(BaseAddress))
            {
                var response = Get($"{BaseAddress}{url}");
                Assert.Equal(expectedStatusCode, response.StatusCode);
            }
        }

        [Fact]
        public void should_support_head_request()
        {
            var serverBuilder = new MockedHttpServerBuilder();
            serverBuilder
                .When(Matchers.Regex(@"/staffs"), HttpMethod.Head)
                .Respond(HttpStatusCode.InternalServerError);

            using (serverBuilder.Build(BaseAddress))
            {
                var response = SendHttpRequest($"{BaseAddress}/staffs", HttpMethod.Head);
                Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
            }
        }

        [Fact]
        public void should_support_is_regex_for_post()
        {
            var serverBuilder = new MockedHttpServerBuilder();
            serverBuilder
                .WhenPost(Matchers.Regex(@"/staffs"))
                .Respond(HttpStatusCode.InternalServerError);

            using (serverBuilder.Build(BaseAddress))
            {
                var data = new {Name = "Staff", Email = "emal@staff.com"};
                var response = Post($"{BaseAddress}/staffs", data);
                Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
            }
        }

        [Fact]
        public void should_support_it_is_star_wildcard()
        {
            var serverBuilder = new MockedHttpServerBuilder();
            serverBuilder
                .WhenGet(Matchers.Wildcard(@"/staff*"))
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
                .WhenGet(Matchers.Wildcard(@"/staffs/?"))
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
                .WhenGet(Matchers.Is("/staff?employeeId=Staff0001"))
                .Respond(HttpStatusCode.InternalServerError);

            using (serverBuilder.Build(BaseAddress))
            {
                var response = Get(RequestUri);
                Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
            }

        }
    }
}