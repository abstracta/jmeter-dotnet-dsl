### CSV as input data for requests

Sometimes is necessary to run the same flow but using different pre-defined data on each request. For example, a common use case is to use a different user (from a given set) in each request.

This can be easily achieved using the provided `CsvDataSet` element. For example, having a file like this one:

```csv
USER,PASS
user1,pass1
user2,pass2
```

You can implement a test plan that tests recurrent login with the two users with something like this:

```cs
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using static Abstracta.JmeterDsl.JmeterDsl;

public class PerformanceTest
{
    [Test]
    public void LoadTest()
    {
        var stats = TestPlan(
            CsvDataSet("users.csv"),
            ThreadGroup(5, 10,
                HttpSampler("http://my.service/login")
                    .Post("{\"${USER}\": \"${PASS}\"", new MediaTypeHeaderValue(MediaTypeNames.Application.Json)),
                HttpSampler("http://my.service/logout")
                    .Method(HttpMethod.Post.Method)
            )
        ).Run();
        Assert.That(stats.Overall.SampleTimePercentile99, Is.LessThan(TimeSpan.FromSeconds(5)));
    }
}
```

::: tip
By default, the CSV file will be opened once and shared by all threads. This means that when one thread reads a CSV line in one iteration, then the following thread reading a line will continue with the following line.

If you want to change this (to share the file per thread group or use one file per thread), then you can use the provided `SharedIn` method like in the following example:

```java
using static Abstracta.JmeterDsl.Core.Configs.DslCsvDataSet;
...
    var stats = TestPlan(
        CsvDataSet("users.csv")
            .SharedIn(Sharing.Thread),
        ThreadGroup(5, 10,
            HttpSampler("http://my.service/login")
                .Post("{\"${USER}\": \"${PASS}\"", new MediaTypeHeaderValue(MediaTypeNames.Application.Json)),
            HttpSampler("http://my.service/logout")
                .Method(HttpMethod.Post.Method)
        )
    ).Run();
    Assert.That(stats.Overall.SampleTimePercentile99, Is.LessThan(TimeSpan.FromSeconds(5)));
```
:::

::: warning
You can use the `RandomOrder()` method to get CSV lines in random order (using [Random CSV Data Set plugin](https://github.com/Blazemeter/jmeter-bzm-plugins/blob/master/random-csv-data-set/RandomCSVDataSetConfig.md)), but this is less performant as getting them sequentially, so use it sparingly.
:::

Check [DslCsvDataSet](/Abstracta.JmeterDsl/Core/Configs/DslCsvDataSet.cs) for additional details and options (like changing delimiter, handling files without headers line, stopping on the end of file, etc.).
