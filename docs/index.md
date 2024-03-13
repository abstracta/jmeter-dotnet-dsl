---
home: true
heroHeight: 68
heroImage: /logo.svg
actions:
  - text: User Guide →
    link: /guide/
features:
- title: 💙 Git, IDE & Programmers Friendly
  details: Simple way of defining performance tests that takes advantage of IDEs autocompletion and inline documentation.
- title: 💪 JMeter ecosystem & community
  details: Use the most popular performance tool and take advantage of the wide support of protocols and tools.
- title: 😎 Built-in features & extensibility
  details: Built-in additional features which ease usage and using it in CI/CD pipelines.
footer: Made by <a href="https://abstracta.us">Abstracta</a> with ❤️ | Apache 2.0 Licensed | Powered by <a href="https://v2.vuepress.vuejs.org/">Vuepress</a>
footerHtml: true
---

## Example

Add the package to your project:

```powershell
dotnet add package Abstracta.JmeterDsl --version 0.5
```

Create performance test:

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
            )
        ).Run();
        Assert.That(stats.Overall.SampleTimePercentile99, Is.LessThan(TimeSpan.FromSeconds(5)));
    }
}
```

> **Java 8+ is required** for test plan execution.

[Here](https://github.com/abstracta/jmeter-dotnet-dsl-sample) is a sample project in case you want to start one from scratch.

