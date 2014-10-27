using System;

namespace SimpleHttpMock
{
    public class ActualRequest
    {
        public string Method { get; set; }
        public dynamic RequestBody { get; set; }
        public Uri RequestUri { get; set; }     

        public static ActualRequest ToRequest()
        {
            return default(ActualRequest);
        }
    }

    public class ActualRequest<T>
    {
        public string Method { get; set; }
        public T RequestBody { get; set; }
        public Uri RequestUri { get; set; }
    }
}