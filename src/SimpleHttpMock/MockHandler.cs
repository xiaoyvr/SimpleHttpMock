using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace SimpleHttpMock
{
    internal class MockHandler : DelegatingHandler
    {
        private RequestBehaviors requestBehaviors;

        public MockHandler(RequestBehaviors requestBehaviors)
        {
            this.requestBehaviors = requestBehaviors;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            var httpResponseMessage = requestBehaviors.CreateResponse(request);
            if (request.Method == HttpMethod.Head)
            {
                httpResponseMessage.Content = null;
            }
            return Task.FromResult(httpResponseMessage);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Reconfigure(IEnumerable<RequestBehavior> behaviors, bool deleteExistingMocks)
        {
            requestBehaviors = deleteExistingMocks
                ? new RequestBehaviors(behaviors)
                : new RequestBehaviors(behaviors.Concat(requestBehaviors.Behaviors));
        }
    }
}