using System;
using System.Collections.Generic;

namespace SimpleHttpMock
{
    public class ActualRequest
    {
        public string Method { get; set; }
        public dynamic RequestBody { get; set; }
        public Uri RequestUri { get; set; }

        public Dictionary<string, string> RequestHeaders { get; set; } = new Dictionary<string, string>();

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