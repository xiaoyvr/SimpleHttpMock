namespace SimpleHttpMock
{
    internal class AlwaysMatchProcessor : IRequestProcessor
    {
        public bool Process(HttpRequestMessageWrapper httpRequestMessageWrapper)
        {
            return true;
        }
    }
}