using System.Net.Http;
using System.Threading.Tasks;

namespace SimpleHttpMock
{
    internal class MockHandler : DelegatingHandler
    {
        private readonly RequestBehaviors requestBehaviors;

        public MockHandler(RequestBehaviors requestBehaviors)
        {
            this.requestBehaviors = requestBehaviors;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<HttpResponseMessage>();
            tcs.SetResult(requestBehaviors.CreateResponse(request));
            return tcs.Task;
        }
    }
}