#### Methods & body

As previously seen, you can do simple gets and posts like in the following snippet:

```cs
HttpSampler("http://my.service") // A simple get
HttpSampler("http://my.service")
    .Post("{\"field\":\"val\"}", new MediaTypeHeaderValue(MediaTypeNames.Application.Json)) // simple post
```

But you can also use additional methods to specify any HTTP method and body:

```cs
HttpSampler("http://my.service")
  .Method(HttpMethod.Put.Method)
  .ContentType(new MediaTypeHeaderValue(MediaTypeNames.Application.Json))
  .Body("{\"field\":\"val\"}")
```
