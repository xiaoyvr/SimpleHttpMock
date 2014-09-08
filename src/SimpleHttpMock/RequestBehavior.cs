using System;
using System.Net;
using System.Text.RegularExpressions;

namespace SimpleHttpMock
{
    class RequestBehavior
    {
        private readonly string method;
        public object Response { get; private set; }
        public Uri Location { get; set; }
        public HttpStatusCode StatusCode { get; private set; }
        public string MatchUri { get; private set; }
        public IRequestProcessor RequestProcessor { get; private set; }

        public RequestBehavior(HttpStatusCode statusCode, string matchUri, string method, IRequestProcessor requestProcessor, object response, Uri location)
        {
            this.method = method;
            Response = response;
            Location = location;
            MatchUri = matchUri;
            StatusCode = statusCode;
            RequestProcessor = requestProcessor;
        }

        public bool Process(HttpRequestMessageWrapper httpRequestMessageWrapper)
        {
            var uriMatched = UriMatched(httpRequestMessageWrapper.RequestUri) && httpRequestMessageWrapper.Method.Equals(method);
            return uriMatched && RequestProcessor.Process(httpRequestMessageWrapper);
        }

        private bool UriMatched(Uri requestUri)
        {
            var matcher = Regex.Match(requestUri.ToString(), @MatchUri, RegexOptions.IgnoreCase);
            return matcher.Success;
        }
    }
}