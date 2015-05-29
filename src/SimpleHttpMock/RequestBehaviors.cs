using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace SimpleHttpMock
{
    class RequestBehaviors
    {
        private readonly IEnumerable<RequestBehavior> behaviors;

        public RequestBehaviors(IEnumerable<RequestBehavior> behaviors)
        {
            this.behaviors = behaviors;
        }

        public HttpResponseMessage CreateResponse(HttpRequestMessage request)
        {
            var httpRequestMessageWrapper = new HttpRequestMessageWrapper(request);
            var requestBehavior = behaviors.FirstOrDefault(behavior => behavior.Process(httpRequestMessageWrapper));

            if (requestBehavior != null)
                return requestBehavior.CreateResponseMessage(request);
            return request.CreateResponse(HttpStatusCode.OK);
        }
    }

}