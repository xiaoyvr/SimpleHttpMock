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
                    case "application/x-www-form-urlencoded":
                        return (T)(object) content.ReadAsStringAsync().Result;
                    case "multipart/form-data":
                        return (T)(object) new MultipartContentProvider(content.ReadAsMultipartAsync().Result);
                }
            }
            return content.ReadAsAsync<T>().Result;
        }
    }
}