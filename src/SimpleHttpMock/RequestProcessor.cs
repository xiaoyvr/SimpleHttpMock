using System;
using System.Net.Http;

namespace SimpleHttpMock
{
    public class HttpRequestMessageWrapper
    {
        private readonly HttpRequestMessage request;
        private object actualRequest;
        public string Method { get; set; }

        public HttpRequestMessageWrapper(HttpRequestMessage request)
        {
            this.request = request;
            RequestUri = request.RequestUri;
            Method = request.Method.Method;
        }

        public ActualRequest<T> GetActualRequest<T>()
        {
            if (actualRequest == null)
            {
                actualRequest = new ActualRequest<T>
                {
                    RequestUri = request.RequestUri,
                    RequestBody = request.Content.Read<T>(),
                };
            }
            return (ActualRequest<T>)actualRequest;
        }

        public Uri RequestUri { get; private set; }
    }

    public class RequestProcessor<T> : IRequestProcessor
    {
        private readonly Func<ActualRequest<T>, bool> matchFunc;
        private readonly Action<ActualRequest<T>> callback;

        public RequestProcessor(Func<ActualRequest<T>, bool> matchFunc, Action<ActualRequest<T>> callback)
        {
            this.matchFunc = matchFunc;
            this.callback = callback;
        }

        public bool Process(HttpRequestMessageWrapper httpRequestMessageWrapper)
        {
            var actualRequest = httpRequestMessageWrapper.GetActualRequest<T>();
            var matched = matchFunc(actualRequest);
            if (matched)
            {
                callback(actualRequest);
                return true;
            }
            return false;
        }
    }
}