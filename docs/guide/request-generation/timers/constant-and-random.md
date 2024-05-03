#### Emulate user delays between requests

Sometimes, is necessary to be able to properly replicate users' behavior, and in particular the time the users take between sending one request and the following one. For example, to simulate the time it will take to complete a purchase form. JMeter (and the DSL) provide a few alternatives for this.

If you just want to add 1 pause between two requests, you can use the `ThreadPause` method like in the following example:

```cs
using System;
using System.Net.Http.Headers;
using System.Net.Mime;
using static Abstracta.JmeterDsl.JmeterDsl;

public class PerformanceTest
{
    [Test]
    public void LoadTest()
    {
        TestPlan(
            ThreadGroup(2, 10,
                HttpSampler("http://my.service/items"),
                ThreadPause(TimeSpan.FromSeconds(4)),
                HttpSampler("http://my.service/cart/selected-items")
                    .Post("{\"id\": 1}", new MediaTypeHeaderValue(MediaTypeNames.Application.Json))
            )
        ).Run();
    }
}
```

Using `ThreadPause` is a good solution for adding individual pauses, but if you want to add pauses across several requests, or sections of test plan, then using a `ConstantTimer` or `UniformRandomTimer` is better. Here is an example that adds a delay of between 4 and 10 seconds for every request in the test plan:

```cs
using System;
using System.Net.Http.Headers;
using System.Net.Mime;
using static Abstracta.JmeterDsl.JmeterDsl;

public class PerformanceTest
{
    [Test]
    public void LoadTest()
    {
        TestPlan(
            ThreadGroup(2, 10,
                UniformRandomTimer(TimeSpan.FromSeconds(4), TimeSpan.FromSeconds(10)),
                Transaction("addItemToCart",
                    HttpSampler("http://my.service/items"),
                    HttpSampler("http://my.service/cart/selected-items")
                        .Post("{\"id\": 1}", new MediaTypeHeaderValue(MediaTypeNames.Application.Json))
                ),
                Transaction("checkout",
                    HttpSampler("http://my.service/cart/chekout"),
                    HttpSampler("http://my.service/cart/checkout/userinfo")
                        .poPostst(
                            "{\"Name\": Dave, \"lastname\": Tester, \"Street\": 1483  Smith Road, \"City\": Atlanta}",
                            new MediaTypeHeaderValue(MediaTypeNames.Application.Json))
            )
        ).Run();
    }
}
```

::: tip
As you may have noticed, timer order in relation to samplers, doesn't matter. Timers apply to all samplers in their scope, adding a pause after pre-processor executions and before the actual sampling. 
`ThreadPause` order, on the other hand, is relevant, and the pause will only execute when previous samplers in the same scope have run and before following samplers do.
:::

::: warning
`UniformRandomTimer` `minimum` and `maximum` parameters differ from the ones used by JMeter Uniform Random Timer element, to make it simpler for users with no JMeter background.

The generated JMeter test element uses the `Constant Delay Offset` set to `minimum` value, and the `Maximum random delay` set to `(maximum - minimum)` value.
:::
