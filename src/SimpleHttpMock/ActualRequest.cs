using System;

namespace SimpleHttpMock
{
    public class ActualRequest<T>
    {
        public string Method { get; set; }
        public T RequestBody { get; set; }
        public Uri RequestUri { get; set; }     
    }
}