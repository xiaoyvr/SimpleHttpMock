using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using Microsoft.Owin.Hosting;
using Owin;

namespace SimpleHttpMock
{
    public sealed class MockedHttpServer : IDisposable
    {
        private readonly DelegatingHandler byPassHandler;
        readonly IDisposable server;

        public MockedHttpServer(DelegatingHandler byPassHandler, string baseAddress, Action<HttpConfiguration> setup = null)
        {
            this.byPassHandler = byPassHandler;
            server = WebApp.Start(baseAddress, b =>
            {
                var configuration = new HttpConfiguration();
                configuration.MessageHandlers.Add(this.byPassHandler);
                setup?.Invoke(configuration);
                b.UseWebApi(configuration);
            });
        }

        public void Dispose()
        {
            server.Dispose();
        }

        internal void ReconfigureBehaviors(IEnumerable<RequestBehavior> behaviors, bool deleteExistingMocks)
        {
            var mockHandler = byPassHandler as MockHandler;
            if (mockHandler == null)
                throw new NotSupportedException("MockedHttpServer has been constructed with handler not allowing reconfiguration");
            mockHandler.Reconfigure(behaviors, deleteExistingMocks);
        }
    }
}