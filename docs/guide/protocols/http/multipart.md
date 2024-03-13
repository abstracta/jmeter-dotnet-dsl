#### Multipart requests

When you need to upload files to an HTTP server or need to send a complex request body, you will in many cases require sending multipart requests. To send a multipart request just use `BodyPart` and `BodyFilePart` methods like in the following example:

```cs
using static Abstracta.JmeterDsl.JmeterDsl;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;

public class PerformanceTest
{
    [Test]
    public void LoadTest()
    {
        TestPlan(
            ThreadGroup(1, 1,
                HttpSampler("https://myservice.com/report"),
                    .Method(HttpMethod.Post.Method)
                    .BodyPart("myText", "Hello World", new MediaTypeHeaderValue(MediaTypeNames.Text.Plain))
                    .BodyFilePart("myFile", "myReport.xml", new MediaTypeHeaderValue(MediaTypeNames.Text.Xml))
            )
        ).Run();
    }
}
```
