SimpleHttpMock
==============

A really simple http mock using self host service. 

### Using Hamcrest Matchers
* **It.IsRegex ** 

        var serverBuilder = new MockedHttpServerBuilder();
        serverBuilder
            .WhenGet(It.IsRegex(@"/(staff)|(user)s"))
            .Respond(HttpStatusCode.InternalServerError);

        using (serverBuilder.Build(BaseAddress))
        {
            var response = Get("http://localhost/users"));
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        }

* **It.IsWildCard**

		
		serverBuilder
		     .WhenGet(It.IsWildcard(@"/staffs/?"))
		     .Respond(HttpStatusCode.Unauthorized);

* **It.Is**

		serverBuilder
		     .WhenGet(It.Is("/staffs"))
		     .Respond(HttpStatusCode.OK);