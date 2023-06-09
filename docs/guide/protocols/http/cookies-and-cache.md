#### Cookies & caching

JMeter DSL automatically adds a cookie manager and cache manager for automatic HTTP cookie and caching handling, emulating a browser behavior. If you need to disable them you can use something like this:

```cs
TestPlan(
    HttpCookies().Disable(),
    HttpCache().Disable(),
    ThreadGroup(2, 10,
        HttpSampler("http://my.service")
    )
)
```
