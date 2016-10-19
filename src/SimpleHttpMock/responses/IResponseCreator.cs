using System.Net;
using System.Net.Http;

namespace SimpleHttpMock.responses
{
    public interface IResponseCreator
    {
        HttpResponseMessage CreateResponseFor(HttpRequestMessage request, HttpStatusCode statusCode);
    }
}