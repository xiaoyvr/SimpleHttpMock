using System;
using System.Net;
using System.Net.Http;

namespace SimpleHttpMock
{
    class RequestBehavior
    {
        private readonly HttpMethod method;
        public object Response { get; private set; }
        public Uri Location { get; set; }
        public HttpStatusCode StatusCode { get; private set; }
        public Func<string, bool> urlMatcher { get; private set; }
        public IRequestProcessor RequestProcessor { get; private set; }

        public RequestBehavior(HttpStatusCode statusCode, Func<string, bool> urlMatcher, HttpMethod method, IRequestProcessor requestProcessor, object response, Uri location)
        {
            this.method = method;
            Response = response;
            Location = location;
            StatusCode = statusCode;
            this.urlMatcher = urlMatcher;
            RequestProcessor = requestProcessor;
        }

        public bool Process(HttpRequestMessageWrapper httpRequestMessageWrapper)
        {
            var pathAndQuery = httpRequestMessageWrapper.RequestUri.PathAndQuery;
            var isUriMatch = urlMatcher(pathAndQuery);
            var isMethodMatch = httpRequestMessageWrapper.Method.Equals(method.ToString());
            return isUriMatch && isMethodMatch && RequestProcessor.Process(httpRequestMessageWrapper);
        }
    }
}