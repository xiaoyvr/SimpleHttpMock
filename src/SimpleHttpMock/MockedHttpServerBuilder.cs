using System;
using System.Collections.Generic;
using System.Linq;
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
            var builder = new RequestBehaviorBuilder(uri,method);
            builders.Add(builder);
            return builder;
        }
    }
}