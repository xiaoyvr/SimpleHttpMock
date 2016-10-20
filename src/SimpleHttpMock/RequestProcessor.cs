using System;

namespace SimpleHttpMock
{
    internal class RequestProcessor<T> : RequestProcessorBase
    {
        private readonly Func<ActualRequest<T>, bool> matchFunc;
        private readonly Action<ActualRequest<T>> callback;

        public RequestProcessor(Func<ActualRequest<T>, bool> matchFunc, Action<ActualRequest<T>> callback)
        {
            this.matchFunc = matchFunc;
            this.callback = callback;
        }
        
        protected override object DoProcess(HttpRequestMessageWrapper httpRequestMessageWrapper)
        {
            var actualRequest = httpRequestMessageWrapper.GetActualRequest<T>();
            var matched = matchFunc(actualRequest);
            if (matched)
            {
                callback(actualRequest);
                return actualRequest;
            }
            return null;
        }
    }

    internal class RequestProcessor : RequestProcessorBase
    {
        private readonly Func<ActualRequest, bool> matchFunc;
        private readonly Action<ActualRequest> callback;

        public RequestProcessor(Func<ActualRequest, bool> matchFunc, Action<ActualRequest> callback)
        {
            this.matchFunc = matchFunc;
            this.callback = callback;
        }

        protected override object DoProcess(HttpRequestMessageWrapper httpRequestMessageWrapper)
        {
            var actualRequest = httpRequestMessageWrapper.GetActualRequest();
            var matched = matchFunc(actualRequest);
            if (matched)
            {
                callback(actualRequest);
                return actualRequest;
            }
            return null;
        }
    }
}