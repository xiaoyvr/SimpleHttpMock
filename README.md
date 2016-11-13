SimpleHttpMock [![Build status](https://ci.appveyor.com/api/projects/status/wpscw2efwccjr0l4?svg=true)](https://ci.appveyor.com/project/xiaoyvr/simplehttpmock)
==============

A really simple http mock using self host service.

### Match by simple url

```cs
var builder = new MockedHttpServerBuilder();
builder
    .WhenGet("/Test")
    .Respond(HttpStatusCode.OK);
using (builder.Build("http://localhost:1122"))
using (var httpClient = new HttpClient())
{
    var response = await httpClient.GetAsync("http://localhost:1122/test");
    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
}
```

### Return some response
```cs
var builder = new MockedHttpServerBuilder();
builder
    .WhenGet("/Test")
    .Respond(HttpStatusCode.OK, new People { Name = "John Doe"});
using (builder.Build("http://localhost:1122"))
using (var httpClient = new HttpClient())
{
    var response = await httpClient.GetAsync("http://localhost:1122/test");
    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    var people = await response.ReadAsAsync<People>()
    Assert.Equal("John Doe", people.Name);
}
```

### Create server first, build behavior afterwards
```cs
using (var server = new MockedHttpServer("http://localhost:1122"))
{
    var builder = new MockedHttpServerBuilder();
    builder.WhenGet("/test").Respond(HttpStatusCode.OK);
    builder.Build(server);
    using (var httpClient = new HttpClient())
    {
        var response = httpClient.GetAsync("http://localhost:1122/test");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
```

### Retrieve request being sent

```cs
var builder = new MockedHttpServerBuilder();
var requestRetriever = builder.WhenGet("/test").Respond(HttpStatusCode.OK).Retrieve();
using (builder.Build("http://localhost:1122"))
{
    using (var httpClient = new HttpClient())
    {
        // no request being sent yet
        Assert.Null(requestRetriever());

        // when send the request
        var response = await httpClient.GetAsync("http://localhost:1122/test1");

        // should get the request by retriever
        var actualRequest = requestRetriever();
        Assert.NotNull(actualRequest);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);                    
        Assert.Equal("GET", actualRequest.Method);
        Assert.Equal("http://localhost:1122/test", actualRequest.RequestUri.ToString());
    }
}
```

### Hamcrest Style Matchers

* **Matchers.Regex**

```cs
 var serverBuilder = new MockedHttpServerBuilder();
 serverBuilder
     .WhenGet(Matchers.Regex(@"/(staff)|(user)s"))
     .Respond(HttpStatusCode.InternalServerError);

 using (serverBuilder.Build(BaseAddress))
 {
     var response = Get("http://localhost/users"));
     Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
 }

```

* **Matchers.WildCard**

```cs	
serverBuilder
    .WhenGet(Matchers.Wildcard(@"/staffs/?"))
    .Respond(HttpStatusCode.Unauthorized);
```

* **Matchers.Is**

```cs
serverBuilder
     .WhenGet(Matchers.Is("/staffs"))
     .Respond(HttpStatusCode.OK);
```


### Other Matchers

```cs	
serverBuilder
    .When(Matchers.Wildcard(@"/staffs/?"), HttpMethod.Patch)
    .Respond(HttpStatusCode.Unauthorized);
```
