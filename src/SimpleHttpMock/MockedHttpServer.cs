using System;
using System.Collections.Generic;
using System.Web.Http;
using Microsoft.Owin.Hosting;
using Owin;

namespace SimpleHttpMock
{
    public sealed class MockedHttpServer : IDisposable
    {
        private readonly MockHandler byPassHandler;
        readonly IDisposable server;

        internal MockedHttpServer(MockHandler byPassHandler, string baseAddress, Action<HttpConfiguration> setup = null)
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
        public MockedHttpServer(string baseAddress, Action<HttpConfiguration> setup = null) 
            : this(new MockHandler(new RequestBehaviors(new RequestBehavior[0])), baseAddress, setup)
        {
        }

        public void Dispose()
        {
            server.Dispose();
        }

        internal void ReconfigureBehaviors(IEnumerable<RequestBehavior> behaviors, bool renew)
        {
            byPassHandler.Reconfigure(behaviors, renew);
        }
    }
}