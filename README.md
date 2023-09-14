![logo](https://raw.githubusercontent.com/abstracta/jmeter-dotnet-dsl/main/docs/.vuepress/public/logo.svg)

Simple .Net API to run performance tests, using [JMeter](http://jmeter.apache.org/) as engine, in a Git and programmers friendly way. 

If you like this project, **please give it a star :star:!** This helps the project be more visible, gain relevance, and encourage us to invest more effort in new features.

[Here](https://abstracta.github.io/jmeter-java-dsl), you can find the Java DSL.

Please join [discord server](https://discord.gg/WNSn5hqmSd) or create GitHub [issues](https://github.com/abstracta/jmeter-dotnet-dsl/issues) and [discussions](https://github.com/abstracta/jmeter-dotnet-dsl/discussions) to be part of the community and clear out doubts, get the latest news, propose ideas, report issues, etc.

## Usage

Add the package to your project:

```powershell
dotnet add package Abstracta.JmeterDsl --version 0.3
``` 

Here is a simple example test using [Nunit](https://nunit.org/)+ with 2 threads/users iterating 10 times each to send HTTP POST requests with a JSON body to `http://my.service`:

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
                HttpSampler("http://my.service")
                    .Post("{\"name\": \"test\"}", new MediaTypeHeaderValue(MediaTypeNames.Application.Json))
            ),
            //this is just to log details of each request stats
            JtlWriter("jtls")
        ).Run();
        Assert.That(stats.Overall.SampleTimePercentile99, Is.LessThan(TimeSpan.FromSeconds(5)));
    }
}
```

> **Java 8+ is required** for test plan execution.

More examples can be found in [tests](Abstracta.JmeterDsl.Tests)

[Here](https://github.com/abstracta/jmeter-dotnet-dsl-sample) is a sample project for reference or for starting new projects from scratch.

> **Tip 1:** When working with multiple samplers in a test plan, specify their names to easily check their respective statistics.

> **Tip 2:** Since JMeter uses [log4j2](https://logging.apache.org/log4j/2.x/), if you want to control the logging level or output, you can use something similar to the tests included [log4j2.xml](Abstracta.JmeterDsl.Tests/log4j2.xml), using "CopyToOutputDirectory" in the project item so the file is available in dotnet build output directory as well (check [Abstracta.JmeterDsl.Test/Abstracta.JmeterDsl.Tests.csproj]).


**Check [here](https://abstracta.github.io/jmeter-dotnet-dsl/) for details on some interesting use cases**, like running tests at scale in [Azure Load Testing](https://azure.microsoft.com/en-us/products/load-testing/), and general usage guides.

## Why?

Check more about the motivation and analysis of alternatives [here](https://abstracta.github.io/jmeter-java-dsl/motivation/)

## Support

Join our [Discord server](https://discord.gg/WNSn5hqmSd) to engage with fellow JMeter DSL enthusiasts, ask questions, and share experiences. Visit [GitHub Issues](https://github.com/abstracta/jmeter-dotnet-dsl/issues) or [GitHub Discussions](https://github.com/abstracta/jmeter-dotnet-dsl/discussions) for bug reports, feature requests and share ideas.

[Abstracta](https://abstracta.us), the main supporter for JMeter DSL development, offers enterprise-level support. Get faster response times, personalized customizations and consulting.

For detailed support information, visit our [Support](https://abstracta.github.io/jmeter-dotnet-dsl/support) page.

## Articles & Talks

Check articles and talks mentioning the Java version [here](https://github.com/abstracta/jmeter-java-dsl#articles--talks).

## Ecosystem

* [Jmeter Java DSL](https://abstracta.github.io/jmeter-java-dsl): Java API which is the base of the .Net API.
* [pymeter](https://github.com/eldaduzman/pymeter): Python API based on JMeter Java DSL that allows Python devs to create and run JMeter test plans.

## Contributing & Requesting features

Currently, the project covers some of the most used features of JMeter and JMeter Java DSL test, but not everything, as we keep improving it to cover more use cases.

We invest in the development of DSL according to the community's (your) interest, which we evaluate by reviewing GitHub stars' evolution, feature requests, and contributions.

To keep improving the DSL we need you to **please create an issue for any particular feature or need that you have**.

We also really appreciate pull requests. Check the [CONTRIBUTING](CONTRIBUTING.md) guide for an explanation of the main library components and how you can extend the library.
