using System;
using System.Net;
using System.Net.Http;

namespace SimpleHttpMock.responses
{
    public class HttpContentResponseCreator : IResponseCreator
    {
        private readonly Func<HttpRequestMessage, HttpContent> _contentFn;

        public HttpContentResponseCreator(Func<HttpRequestMessage, HttpContent> contentFn)
        {
            _contentFn = contentFn;
        }

        public HttpResponseMessage CreateResponseFor(HttpRequestMessage request, HttpStatusCode statusCode)
        {
            return new HttpResponseMessage { Content = _contentFn(request), StatusCode = statusCode };
        }
    }
}