## Simple HTTP test plan

To generate HTTP requests just use provided `HttpSampler`.

The following example uses 2 threads (concurrent users) that send 10 HTTP GET requests each to `http://my.service`.

Additionally, it logs collected statistics (response times, status codes, etc.) to a file (for later analysis if needed) and checks that the response time 99 percentile is less than 5 seconds.

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
            ),
            //this is just to log details of each request stats
            JtlWriter("jtls")
        ).Run();
        Assert.That(stats.Overall.SampleTimePercentile99, Is.LessThan(TimeSpan.FromSeconds(5)));
    }
}
```

::: tip
When working with multiple samplers in a test plan, specify their names (eg: `HttpSampler("home", "http://my.service")`) to easily check their respective statistics.
:::

::: tip
JMeter .Net DSL uses Java for executing JMeter test plans. If you need to tune JVM parameters, for example for specifying maximum heap memory size, you can use `EmbeddedJMeterEngine` and the `JvmArgs` method like in the following example:

```cs
using Abstracta.JmeterDsl.Core.Engines;
...
var stats = TestPlan(
    ThreadGroup(2, 10,
        HttpSampler("http://my.service")
    )
).RunIn(new EmbeddedJmeterEngine()
    .JvmArgs("-Xmx4g")
);
```
:::

::: tip
Since JMeter uses [log4j2](https://logging.apache.org/log4j/2.x/), if you want to control the logging level or output, you can use something similar to this [log4j2.xml](Abstracta.JmeterDsl.Tests/log4j2.xml), using "CopyToOutputDirectory" in the project item, so the file is available in dotnet build output directory as well (check [Abstracta.JmeterDsl.Test/Abstracta.JmeterDsl.Tests.csproj]).
:::

::: tip
Depending on the test framework you use, and the way you run your tests, you might be able to see JMeter logs and output in real-time, at the end of the test, or not see them at all. This is not something we can directly control in JMeter DSL, and heavily depends on the dotnet environment and testing framework implementation.

When using Nunit, to get real-time console output from JMeter you might want to run your tests with something like `dotnet test -v n` and add the following code to your tests:

```cs
private TextWriter? originalConsoleOut;

// Redirecting output to progress to get live stdout with nunit.
// https://github.com/nunit/nunit3-vs-adapter/issues/343
// https://github.com/nunit/nunit/issues/1139
[SetUp]
public void SetUp()
{
    originalConsoleOut = Console.Out;
    Console.SetOut(TestContext.Progress);
}

[TearDown]
public void TearDown()
{
    Console.SetOut(originalConsoleOut!);
}
```
:::

::: tip
Keep in mind that you can use .Net programming to modularize and create abstractions which allow you to build complex test plans that are still easy to read, use and maintain. [Here is an example](https://github.com/abstracta/jmeter-java-dsl/issues/26#issuecomment-953783407) of some complex abstraction built using Java features (you can easily extrapolate to .Net) and the DSL.
:::

Check [HTTP performance testing](./protocols/http/index#http) for additional details while testing HTTP services.