using System;
using System.Net.Http;
using System.Web.Http.SelfHost;

namespace SimpleHttpMock
{
    public class MockedHttpServer : IDisposable
    {
        private readonly HttpSelfHostServer httpSelfHostServer;

        public MockedHttpServer(DelegatingHandler byPassHandler, string baseAddress, Action<HttpSelfHostConfiguration> setup = null)
        {
            var config = new HttpSelfHostConfiguration(baseAddress);
            config.MessageHandlers.Add(byPassHandler);
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
    }
}