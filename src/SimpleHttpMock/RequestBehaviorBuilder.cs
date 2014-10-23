using System;
using System.Net;
using System.Net.Http;

namespace SimpleHttpMock
{
    public class RequestBehaviorBuilder
    {
        private readonly Func<string, bool> urlMatcher;
        private readonly HttpMethod method;

        private HttpStatusCode statusCode;

        private IRequestProcessor processor;

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
            Response = response;
            Location = location;
            return this;
        }

        public RequestBehaviorBuilder RespondContent(HttpStatusCode httpStatusCode, HttpContent content, Uri location = null)
        {
            statusCode = httpStatusCode;
            Response = content;
            Location = location;
            return this;
        }

        protected object Response = string.Empty;

        protected Uri Location = default(Uri);

        internal RequestBehavior Build()
        {
            return new RequestBehavior(statusCode, urlMatcher, method, processor?? new AlwaysMatchProcessor(),Response, Location);
        }
    }
}