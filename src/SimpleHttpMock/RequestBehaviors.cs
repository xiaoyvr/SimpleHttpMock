using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;

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
            {
                HttpResponseMessage httpResponseMessage;
                var httpContent = requestBehavior.Response as HttpContent;

                if (httpContent != null)
                {
                    var responseMessage = new HttpResponseMessage
                    {
                        Content = httpContent,
                        StatusCode = requestBehavior.StatusCode
                    };
                    httpResponseMessage = responseMessage;
                }
                else
                {
                    httpResponseMessage = request.CreateResponse(requestBehavior.StatusCode, requestBehavior.Response);
                }

                var httpHeaders = requestBehavior.Headers;
                if (httpHeaders != null && httpHeaders.Count > 0)
                {
                    httpHeaders.ToList().ForEach(header=>httpResponseMessage.Headers.Add(header.Key,header.Value));
                }

                httpResponseMessage.Headers.Location = requestBehavior.Location;
                return httpResponseMessage;
            }
            return request.CreateResponse(HttpStatusCode.OK);
        }
    }

}