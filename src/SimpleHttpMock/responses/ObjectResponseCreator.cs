using System;
using System.Net;
using System.Net.Http;

namespace SimpleHttpMock.responses
{
    public class ObjectResponseCreator : IResponseCreator
    {
        private readonly object content;

        public ObjectResponseCreator(object content)
        {
            this.content = content;
        }

        public HttpResponseMessage CreateResponseFor(HttpRequestMessage request, HttpStatusCode statusCode)
        {
            return request.CreateResponse(statusCode, content);
        }
    }
}