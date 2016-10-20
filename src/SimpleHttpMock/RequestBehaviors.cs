using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace SimpleHttpMock
{
    internal class RequestBehaviors
    {
        private readonly RequestBehavior[] behaviors;

        public IEnumerable<RequestBehavior> Behaviors => behaviors;

        public RequestBehaviors(IEnumerable<RequestBehavior> behaviors)
        {
            this.behaviors = behaviors.ToArray();
        }

        public HttpResponseMessage CreateResponse(HttpRequestMessage request)
        {
            var httpRequestMessageWrapper = new HttpRequestMessageWrapper(request);
            var requestBehavior = Behaviors.FirstOrDefault(behavior => behavior.Process(httpRequestMessageWrapper));

            if (requestBehavior != null)
                return requestBehavior.CreateResponseMessage(request);
            return request.CreateResponse(HttpStatusCode.NotFound);
        }
    }

}