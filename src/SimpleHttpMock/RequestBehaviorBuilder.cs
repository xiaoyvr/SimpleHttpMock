using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using SimpleHttpMock.responses;

namespace SimpleHttpMock
{
    public class RequestBehaviorBuilder
    {
        private readonly Func<string, bool> urlMatcher;
        private readonly HttpMethod method;

        private HttpStatusCode statusCode;

        private IRequestProcessor processor;

        private IDictionary<string, string> headers;

        public RequestBehaviorBuilder(Func<string, bool> urlMatcher, HttpMethod method)
        {
            this.method = method;
            this.urlMatcher = urlMatcher;
        }

        public RequestBehaviorBuilder WithRequest<TModel>(
            Action<ActualRequest<TModel>> action, 
            TModel obj,
            Func<ActualRequest<TModel>, bool> matchFunc = null)
        {
            processor = new RequestProcessor<TModel>(matchFunc?? (r => true), action);
            return this;
        }

        public RequestBehaviorBuilder WithRequest(Action<ActualRequest> action, Func<ActualRequest, bool> matchFunc = null)
        {
            processor = new RequestProcessor(matchFunc ?? (r => true), action);
            return this;
        }

        public RequestBehaviorBuilder WithMultipartRequest(Action<ActualRequest<MultipartContentProvider>> action)
        {
            processor = new RequestProcessor<MultipartContentProvider>((r => true), action);
            return this;
        }

        public RequestBehaviorBuilder WithRequest<TModel>(
            Action<ActualRequest<TModel>> action, 
            Func<ActualRequest<TModel>, bool> matchFunc = null)
        {
            processor = new RequestProcessor<TModel>(matchFunc?? (r => true), action);
            return this;
        }

        public RequestBehaviorBuilder Respond(HttpStatusCode httpStatusCode)
        {
            statusCode = httpStatusCode;
            return this;
        }

        public RequestBehaviorBuilder Respond(HttpStatusCode httpStatusCode, object response, Uri location = null)
        {
            statusCode = httpStatusCode;
            Response = new ObjectResponseCreator(response);
            Location = location;
            return this;
        }

        [Obsolete("This method creates mock that can be used only once, because after first call, the content instance is disposed. Please use a safer version of this method: RespondContent(code, request => new StringContent('my content'), location)")]
        public RequestBehaviorBuilder RespondContent(HttpStatusCode httpStatusCode, HttpContent content, Uri location = null)
        {
            statusCode = httpStatusCode;
            Response = new HttpContentResponseCreator(request => content);
            Location = location;
            return this;
        }

        public RequestBehaviorBuilder RespondContent(HttpStatusCode httpStatusCode, Func<HttpRequestMessage,HttpContent> contentFn, Uri location = null)
        {
            statusCode = httpStatusCode;
            Response = new HttpContentResponseCreator(contentFn);
            Location = location;
            return this;
        }

        public RequestBehaviorBuilder RespondHeaders(dynamic headers)
        {
            this.headers =
                JsonConvert.DeserializeObject<Dictionary<string, string>>(JsonConvert.SerializeObject(headers));
            return this;
        }

        protected IResponseCreator Response = new ObjectResponseCreator(string.Empty);

        protected Uri Location = default(Uri);

        internal RequestBehavior Build()
        {
            return new RequestBehavior(statusCode, urlMatcher, method, processor ?? new AlwaysMatchProcessor(), Response, Location, headers);
        }
    }
}