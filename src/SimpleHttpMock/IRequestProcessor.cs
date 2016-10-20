using System;
using System.Collections.Generic;

namespace SimpleHttpMock
{
    public interface IRequestProcessor
    {
        bool Process(HttpRequestMessageWrapper httpRequestMessageWrapper);
        object Match { get; set; }
        List<object> Matches { get; set; }
    }
}