using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using SimpleHttpMock.responses;

namespace SimpleHttpMock
{
    class RequestBehavior
    {
        private readonly HttpMethod method;
        public IResponseCreator ResponseCreator { get; private set; }
        public Uri Location { get; set; }
        public HttpStatusCode StatusCode { get; private set; }
        public Func<string, bool> urlMatcher { get; private set; }
        public IRequestProcessor RequestProcessor { get; private set; }
        public IDictionary<string, string> Headers { get; private set; }


        public RequestBehavior(HttpStatusCode statusCode, Func<string, bool> urlMatcher, HttpMethod method, IRequestProcessor requestProcessor, IResponseCreator responseCreator, Uri location, IDictionary<string, string> headers)
        {
            this.method = method;
            ResponseCreator = responseCreator;
            Location = location;
            StatusCode = statusCode;
            this.urlMatcher = urlMatcher;
            RequestProcessor = requestProcessor;
            Headers = headers;
        }

        public bool Process(HttpRequestMessageWrapper httpRequestMessageWrapper)
        {
            var pathAndQuery = httpRequestMessageWrapper.RequestUri.PathAndQuery;
            var isUriMatch = urlMatcher(pathAndQuery) || urlMatcher(Uri.UnescapeDataString(pathAndQuery));
            var isMethodMatch = httpRequestMessageWrapper.Method.Equals(method.ToString());
            RequestProcessor.Match = null;
            return isUriMatch && isMethodMatch && RequestProcessor.Process(httpRequestMessageWrapper);
        }

        public HttpResponseMessage CreateResponseMessage(HttpRequestMessage request)
        {
            HttpResponseMessage httpResponseMessage = ResponseCreator.CreateResponseFor(request,StatusCode);

            if (Headers != null && Headers.Count > 0)
                Headers.ToList().ForEach(header => httpResponseMessage.Headers.Add(header.Key, header.Value));

            httpResponseMessage.Headers.Location = Location;
            return httpResponseMessage;
        }
    }
}