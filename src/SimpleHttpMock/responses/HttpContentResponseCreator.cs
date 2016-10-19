using System;
using System.Net;
using System.Net.Http;

namespace SimpleHttpMock.responses
{
    public class HttpContentResponseCreator : IResponseCreator
    {
        private readonly Func<HttpRequestMessage, HttpContent> contentFn;

        public HttpContentResponseCreator(Func<HttpRequestMessage, HttpContent> contentFn)
        {
            this.contentFn = contentFn;
        }

        public HttpResponseMessage CreateResponseFor(HttpRequestMessage request, HttpStatusCode statusCode)
        {
            return new HttpResponseMessage { Content = contentFn(request), StatusCode = statusCode };
        }
    }
}