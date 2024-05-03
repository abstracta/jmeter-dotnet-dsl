### Group requests

Sometimes, is necessary to be able to group requests which constitute different steps in a test. For example, to separate necessary requests to do a login from the ones used to add items to the cart and the ones to do a purchase. JMeter (and the DSL) provide Transaction Controllers for this purpose, here is an example:

```cs
using System.Net.Http.Headers;
using System.Net.Mime;
using static Abstracta.JmeterDsl.JmeterDsl;

public class PerformanceTest
{
    [Test]
    public void TestTransactions()
    {
        TestPlan(
            ThreadGroup(2, 10,
                Transaction("login",
                    HttpSampler("http://my.service"),
                    HttpSampler("http://my.service/login")
                        .Post("user=test&password=test", new MediaTypeHeaderValue(MediaTypeNames.Application.FormUrlEncoded))
                ),
                Transaction("checkout",
                    HttpSampler("http://my.service/items"),
                    HttpSampler("http://my.service/cart/items")
                        .poPostst("{\"id\": 1}", new MediaTypeHeaderValue(MediaTypeNames.Application.Json))
            )
        ).Run();
    }
}
```

This will provide additional sample results for each transaction, which contain the aggregate metrics for containing requests, allowing you to focus on the actual flow steps instead of each particular request.

If you don't want to generate additional sample results (and statistics), and want to group requests for example to apply a given timer, config, assertion, listener, pre- or post-processor, then you can use `SimpleController` like in following example:

```cs
using static Abstracta.JmeterDsl.JmeterDsl;

public class PerformanceTest
{
    [Test]
    public void TestTransactions()
    {
        TestPlan(
            ThreadGroup(2, 10,
                simpleController("login",
                    HttpSampler("http://my.service"),
                    HttpSampler("http://my.service/users")
                    ResponseAssertion()
                        .ContainsSubstrings("OK")
                )
            )
        ).Run();
    }
}
```

You can even use `TransactionController` and `SimpleController` to easily modularize parts of your test plan into Java methods (or classes) like in this example:

```cs
using System.Net.Http.Headers;
using System.Net.Mime;
using static Abstracta.JmeterDsl.JmeterDsl;

public class PerformanceTest
{
    private DslTransactionController Login(string baseUrl) =>
        Transaction("login",
            HttpSampler(baseUrl),
            HttpSampler(baseUrl + "/login")
                .Post("user=test&password=test", new MediaTypeHeaderValue(MediaTypeNames.Application.FormUrlEncoded))
        );

    private DslTransactionController AddItemToCart(string baseUrl) =>
        Transaction("addItemToCart",
            HttpSampler(baseUrl + "/items"),
            HttpSampler(baseUrl + "/cart/items")
                .Post("{\"id\": 1}", new MediaTypeHeaderValue(MediaTypeNames.Application.Json))
        );
    
    [Test]
    public void TestTransactions()
    {
        var baseUrl = "http://my.service";
        TestPlan(
            ThreadGroup(2, 10,
                Login(baseUrl),
                AddItemToCart(baseUrl)
            )
        ).Run();
    }
}
```
