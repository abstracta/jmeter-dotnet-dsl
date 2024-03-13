#### Parameters

In many cases, you will need to specify some URL query string parameters or URL encoded form bodies. For these cases, you can use `Param` method as in the following example:

```cs
using static Abstracta.JmeterDsl.JmeterDsl;
using System.Net.Http;

public class PerformanceTest
{
    [Test]
    public void LoadTest()
    {
        var baseUrl = "https://myservice.com/products";
        TestPlan(
            ThreadGroup(1, 1,
                // GET https://myservice.com/products?name=iron+chair
                HttpSampler("GetIronChair", baseUrl)
                    .Param("name", "iron chair"),
                /*
                * POST https://myservice.com/products
                * Content-Type: application/x-www-form-urlencoded
                * 
                * name=wooden+chair
                */
                HttpSampler("CreateWoodenChair", baseUrl)
                    .Method(HttpMethod.Post.Method) // POST 
                    .Param("name", "wooden chair")
            )
        ).Run();
    }
}
```

::: tip
JMeter automatically URL encodes parameters, so you don't need to worry about special characters in parameter names or values.

If you want to use some custom encoding or have an already encoded value that you want to use, then you can use `RawParam` method instead which does not apply any encoding to the parameter name or value, and send it as is.
:::