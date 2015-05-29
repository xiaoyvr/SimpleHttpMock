using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http.SelfHost;

namespace SimpleHttpMock
{
    public class MockedHttpServer : IDisposable
    {
        private readonly HttpSelfHostServer httpSelfHostServer;
        private readonly DelegatingHandler _byPassHandler;

        public MockedHttpServer(DelegatingHandler byPassHandler, string baseAddress, Action<HttpSelfHostConfiguration> setup = null)
        {
            _byPassHandler = byPassHandler;
            var config = new HttpSelfHostConfiguration(baseAddress);
            config.MessageHandlers.Add(_byPassHandler);
            if (setup != null)
            {
                setup(config);
            }
            httpSelfHostServer = new HttpSelfHostServer(config);
            httpSelfHostServer.OpenAsync().Wait();
        }

        public void Dispose()
        {
            httpSelfHostServer.CloseAsync().Wait();
        }

        internal void ReconfigureBehaviors(IEnumerable<RequestBehavior> behaviors, bool deleteExistingMocks)
        {
            var mockHandler = _byPassHandler as MockHandler;
            if (mockHandler == null)
                throw new NotSupportedException("MockedHttpServer has been constructed with handler not allowing reconfiguration");
            mockHandler.Reconfigure(behaviors, deleteExistingMocks);
        }
    }
}