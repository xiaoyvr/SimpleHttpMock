using System;
using System.Net;
using System.Net.Http;

namespace SimpleHttpMock.responses
{
    public class ObjectResponseCreator : IResponseCreator
    {
        private readonly object _content;

        public ObjectResponseCreator(object content)
        {
            _content = content;
        }

        public HttpResponseMessage CreateResponseFor(HttpRequestMessage request, HttpStatusCode statusCode)
        {
            return request.CreateResponse(statusCode, _content);
        }
    }
}