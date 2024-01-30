#### Iterating a fixed number of times

In simple scenarios where you just want to execute a fixed number of times, within a thread group iteration, a given part of the test plan, you can just use `forLoopController` (which uses [JMeter Loop Controller component](https://jmeter.apache.org/usermanual/component_reference.html#Loop_Controller)) as in the following example:

```cs
using static Abstracta.JmeterDsl.JmeterDsl;

public class PerformanceTest
{
    [Test]
    public void LoadTest()
    {
        var stats = TestPlan(
            ThreadGroup(2, 10,
                ForLoopController(5,
                    HttpSampler("http://my.service/accounts")
                )
            )
        ).Run();
        Assert.That(stats.Overall.SampleTimePercentile99, Is.LessThan(TimeSpan.FromSeconds(5)));
    }
}
```

This will result in 10 * 5 = 50 requests to the given URL for each thread in the thread group.

::: tip
JMeter automatically generates a variable `__jm__<loopName>__idx` with the current index of for loop iteration (starting with 0) which you can use in children elements. The default name for the for loop controller, when not specified, is `for`.
:::

Check [ForLoopController](/Abstracta.JmeterDsl/Core/Controllers/ForLoopController.cs) for more details.
