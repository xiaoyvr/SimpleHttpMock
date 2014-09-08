using System.Linq;
using System.Net.Http;

namespace SimpleHttpMock
{
    public class MultipartContentProvider
    {
        private readonly MultipartMemoryStreamProvider streamProvider;

        public MultipartContentProvider(MultipartMemoryStreamProvider streamProvider)
        {
            this.streamProvider = streamProvider;
        }

        public T Get<T>(string name)
        {
            var content = streamProvider.Contents.FirstOrDefault(c => c.Headers.ContentDisposition.Name == name);
            return content.ReadAsAsync<T>().Result;
        }
    }
}