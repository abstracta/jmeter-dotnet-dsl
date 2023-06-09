### Log requests and responses

The main mechanism provided by JMeter (and `Abstracta.JmeterDsl`) to get information about generated requests, responses, and associated metrics is through the generation of JTL files.

This can be easily achieved by using provided `JtlWriter` like in this example:

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
            JtlWriter("jtls")
        ).Run();
    }
}
```

::: tip
By default, `JtlWriter` will write the most used information to evaluate the performance of the tested service. If you want to trace all the information of each request you may use `JtlWriter` with the `WithAllFields()` option. Doing this will provide all the information at the cost of additional computation and resource usage (fewer resources for actual load testing). You can tune which fields to include or not with `JtlWriter` and only log what you need, check [JtlWriter](/Abstracta.JmeterDsl/Core/Listeners/JtlWriter.cs) for more details.
:::

::: tip
`JtlWriter` will automatically generate `.jtl` files applying this format: `<yyyy-MM-dd HH-mm-ss> <UUID>.jtl`.

If you need a specific file name, for example for later postprocessing logic (eg: using CI build ID), you can specify it by using `JtlWriter(directory, fileName)`.

When specifying the file name, make sure to use unique names, otherwise, the JTL contents may be appended to previous existing jtl files.
:::

An additional option, specially targeted towards logging sample responses, is `ResponseFileSaver` which automatically generates a file for each received response. Here is an example:

```cs
using static Abstracta.JmeterDsl.JmeterDsl;

public class PerformanceTest
{
    [Test]
    public void LoadTest()
    {
        TestPlan(
            ThreadGroup(2, 10,
                HttpSampler("http://my.service")
            ),
            ResponseFileSaver(DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss").Replace(":", "-") + "-response")
        ).Run();
    }
}
```

Check [ResponseFileSaver](/Abstracta.JmeterDsl/Core/Listeners/ResponseFileSaver.cs) for more details.
