SimpleHttpMock
==============

A really simple http mock using self host service. 

### Using Hamcrest Matchers

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


### Other methods

```cs	
serverBuilder
    .When(Matchers.Wildcard(@"/staffs/?"), HttpMethod.Patch)
    .Respond(HttpStatusCode.Unauthorized);
```

