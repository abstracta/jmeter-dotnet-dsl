#### Regular expressions extraction

Here is an example with JMeter DSL using regular expressions:

```cs
using System.Net.Http.Headers;
using System.Net.Mime;

using static Abstracta.JmeterDsl.JmeterDsl;

public class PerformanceTest
{
    [Test]
    public void LoadTest()
    {
        var stats = TestPlan(
            ThreadGroup(2, 10,
                HttpSampler("http://my.service/accounts")
                    .Post("{\"name\": \"John Doe\"}", new MediaTypeHeaderValue(MediaTypeNames.Application.Json))
                    .Children(
                        RegexExtractor("ACCOUNT_ID", "\"id\":\"([^\"]+)\"")
                    ),
                HttpSampler("http://my.service/accounts/${ACCOUNT_ID}")
            )
        ).Run();
        Assert.That(stats.Overall.SampleTimePercentile99, Is.LessThan(TimeSpan.FromSeconds(5)));
    }
}
```

Check [DslRegexExtractor](/Abstracta.JmeterDsl/Core/PostProcessors/DslRegexExtractor.cs) for more details and additional options.
