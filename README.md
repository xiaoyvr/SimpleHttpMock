SimpleHttpMock
==============

A really simple http mock using self host service. 

### Using Hamcrest Matchers

* **It.IsRegex**

```cs
 var serverBuilder = new MockedHttpServerBuilder();
 serverBuilder
     .WhenGet(It.IsRegex(@"/(staff)|(user)s"))
     .Respond(HttpStatusCode.InternalServerError);

 using (serverBuilder.Build(BaseAddress))
 {
     var response = Get("http://localhost/users"));
     Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
 }

```

* **It.IsWildCard**

```cs	
serverBuilder
    .WhenGet(It.IsWildcard(@"/staffs/?"))
    .Respond(HttpStatusCode.Unauthorized);
```

* **It.Is**

```cs
serverBuilder
     .WhenGet(It.Is("/staffs"))
     .Respond(HttpStatusCode.OK);
```


### Other methods

```cs	
serverBuilder
    .When(It.IsWildcard(@"/staffs/?"), HttpMethod.Patch)
    .Respond(HttpStatusCode.Unauthorized);
```

