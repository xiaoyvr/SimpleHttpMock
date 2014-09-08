namespace SimpleHttpMock
{
    public interface IRequestProcessor
    {
        bool Process(HttpRequestMessageWrapper httpRequestMessageWrapper);
    }
}