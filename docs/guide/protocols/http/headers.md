#### Headers

You might have already noticed in some of the examples that we have shown, some ways to set some headers. For instance, in the following snippet, `Content-Type` header is being set in two different ways:

```cs
HttpSampler("http://my.service")
  .Post("{\"field\":\"val\"}", new MediaTypeHeaderValue(MediaTypeNames.Application.Json))
HttpSampler("http://my.service")
  .ContentType(new MediaTypeHeaderValue(MediaTypeNames.Application.Json))
```

These are handy methods to specify the `Content-Type` header, but you can also set any header on a particular request using provided `Header` method, like this:

```cs
HttpSampler("http://my.service")
  .Header("X-First-Header", "val1")
  .Header("X-Second-Header", "val2")
```

Additionally, you can specify headers to be used by all samplers in a test plan, thread group, transaction controllers, etc. For this, you can use `HttpHeaders` like this:

```cs
TestPlan(
    ThreadGroup(2, 10,
        HttpHeaders()
          .Header("X-Header", "val1"),
        HttpSampler("http://my.service"),
        HttpSampler("http://my.service/users")
    )
).Run();
```
