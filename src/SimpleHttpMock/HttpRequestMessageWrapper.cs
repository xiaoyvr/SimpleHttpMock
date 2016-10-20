using System;
using System.Dynamic;
using System.Net.Http;
using Newtonsoft.Json;

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
                    Method = request.Method.ToString()
                };
            }
            return (ActualRequest<T>)actualRequest;
        }

        public ActualRequest GetActualRequest()
        {
            if (actualRequest == null)
            {
                var content = request.Content.ReadAsStringAsync().Result;                
                actualRequest = new ActualRequest
                {
                    RequestUri = request.RequestUri,
                    RequestBody = JsonConvert.DeserializeObject<dynamic>(content),
                    Method = request.Method.ToString()
                };
            }
            return (ActualRequest)actualRequest;
        }

        public Uri RequestUri { get; private set; }
    }
}