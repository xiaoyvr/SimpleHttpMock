using System.Net.Http;

namespace SimpleHttpMock
{
    public static class CustomHttpContentExtension
    {
        public static T Read<T>(this HttpContent content)
        {
            var contentType = content.Headers.ContentType;
            if (contentType != null)
            {
                switch (contentType.MediaType)
                {
                    case "multipart/form-data":
                        return (T)(object) new MultipartContentProvider(content.ReadAsMultipartAsync().Result);
                    default:
                        return (T)(object)content.ReadAsStringAsync().Result;
                }
            }
            return content.ReadAsAsync<T>().Result;
        }
    }
}