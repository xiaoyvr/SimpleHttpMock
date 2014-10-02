using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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

        public RequestBehaviorBuilder WhenGet(string uri)
        {
            return WhenGet(Matchers.Is(uri));
        }

        public RequestBehaviorBuilder WhenGet(Func<string, bool> urlMatcher)
        {
            return CreateRequestBehaviorBuilder(urlMatcher, HttpMethod.Get);
        }

        public RequestBehaviorBuilder WhenPost(string uri)
        {
            return WhenPost(Matchers.Is(uri));
        }

        public RequestBehaviorBuilder WhenPost(Func<string, bool> urlMatcher)
        {
            return CreateRequestBehaviorBuilder(urlMatcher, HttpMethod.Post);
        }

        public RequestBehaviorBuilder WhenPut(string uri)
        {
            return WhenPut(Matchers.Is(uri));
        }

        public RequestBehaviorBuilder WhenPut(Func<string, bool> urlMatcher)
        {
            return CreateRequestBehaviorBuilder(urlMatcher, HttpMethod.Put);
        }

        public RequestBehaviorBuilder WhenDelete(string uri)
        {
            return WhenDelete(Matchers.Is(uri));
        }

        public RequestBehaviorBuilder WhenDelete(Func<string, bool> urlMatcher)
        {
            return CreateRequestBehaviorBuilder(urlMatcher, HttpMethod.Delete);
        }

        public RequestBehaviorBuilder When(Func<string, bool> urlMatcher, HttpMethod httpMethod)
        {
            return CreateRequestBehaviorBuilder(urlMatcher, httpMethod);
        }

        private RequestBehaviorBuilder CreateRequestBehaviorBuilder(Func<string, bool> urlMatcher, HttpMethod httpMethod)
        {
            var builder = new RequestBehaviorBuilder(urlMatcher, httpMethod);
            builders.Add(builder);
            return builder;
        }
    }
}