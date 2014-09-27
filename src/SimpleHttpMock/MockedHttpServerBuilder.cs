using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Http.SelfHost;

namespace SimpleHttpMock
{
    public class MockedHttpServerBuilder
    {
        private readonly List<RequestBehaviorBuilder> builders = new List<RequestBehaviorBuilder>();
        public MockedHttpServer Build(string baseAddress, Action<HttpSelfHostConfiguration> setup = null)
        {
            var requestBehaviors = new RequestBehaviors(builders.Select(b => b.Build()));
            var handler = new MockHandler(requestBehaviors);
            return new MockedHttpServer(handler, baseAddress, setup);
        }

        public RequestBehaviorBuilder WhenPost(string uri)
        {
            return CreateRequestBehaviorBuilder(uri, "POST");
        }

        public RequestBehaviorBuilder WhenGet(string uri)
        {
            return CreateRequestBehaviorBuilder(uri,"GET");
        }

        public RequestBehaviorBuilder WhenGet(Func<string, bool> processerMatcher)
        {
            var builder = new RequestBehaviorBuilder(processerMatcher, "GET");
            builders.Add(builder);
            return builder;
        }

        public RequestBehaviorBuilder WhenPut(string uri)
        {
            return CreateRequestBehaviorBuilder(uri,"PUT");
        }

        public RequestBehaviorBuilder WhenDelete(string uri)
        {
            return CreateRequestBehaviorBuilder(uri, "DELETE");
        }

        private RequestBehaviorBuilder CreateRequestBehaviorBuilder(string uri, string method)
        {
            var urlMatcher = It.IsRegex(Regex.Escape(uri));
            var builder = new RequestBehaviorBuilder(urlMatcher,method);
            builders.Add(builder);
            return builder;
        }
    }
}