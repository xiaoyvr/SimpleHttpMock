using System.Collections.Generic;

namespace SimpleHttpMock
{
    public abstract class RequestProcessorBase : IRequestProcessor
    {
        public bool Process(HttpRequestMessageWrapper httpRequestMessageWrapper)
        {
            Match = null;
            Match = DoProcess(httpRequestMessageWrapper);
            if (Match != null)
            {
                Matches.Add(Match);
            }
            return Match != null;
        }
        protected abstract object DoProcess(HttpRequestMessageWrapper httpRequestMessageWrapper);
        public object Match { get;  set; }
        public List<object> Matches { get; set; } = new List<object>();
    }

    internal class AlwaysMatchProcessor : RequestProcessorBase
    {
        protected override object DoProcess(HttpRequestMessageWrapper httpRequestMessageWrapper)
        {
            return httpRequestMessageWrapper.GetActualRequest();
        }        
    }

    internal class AlwaysMatchProcessor<T> : RequestProcessorBase
    {
        protected override object DoProcess(HttpRequestMessageWrapper httpRequestMessageWrapper)
        {
            return httpRequestMessageWrapper.GetActualRequest<T>();
        }
    }
}