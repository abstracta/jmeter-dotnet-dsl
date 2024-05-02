### Check for expected response

By default, JMeter marks any HTTP request with a fail response code (4xx or 5xx) as failed, which allows you to easily identify when some request unexpectedly fails. But in many cases, this is not enough or desirable, and you need to check for the response body (or some other field) to contain (or not) a certain string.

This is usually accomplished in JMeter with the usage of Response Assertions, which provides an easy and fast way to verify that you get the proper response for each step of the test plan, marking the request as a failure when the specified condition is not met.

Here is an example of how to specify a response assertion in JMeter DSL:

```cs
using static Abstracta.JmeterDsl.JmeterDsl;

public class PerformanceTest
{
    [Test]
    public void LoadTest()
    {
        var stats = TestPlan(
            ThreadGroup(2, 10,
                HttpSampler("http://my.service")
                    .Children(
                        ResponseAssertion().ContainsSubstrings("OK")
                    )
            )
        ).Run();
        Assert.That(stats.Overall.SampleTimePercentile99, Is.LessThan(TimeSpan.FromSeconds(5)));
    }
}
```

Check [Response Assertion](/Abstracta.JmeterDsl/Core/Assertions/DslResponseAssertion.cs) for more details and additional options.
