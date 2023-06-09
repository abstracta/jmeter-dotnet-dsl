using System.Net.Http.Headers;
using System.Net.Mime;
using WireMock.FluentAssertions;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace Abstracta.JmeterDsl.Http
{
    using static JmeterDsl;

    public class DslHttpSamplerTest
    {
        private WireMockServer _wiremock;

        [SetUp]
        public void SetUp()
        {
            _wiremock = WireMockServer.Start();
            _wiremock.Given(Request.Create().WithPath("/"))
                .RespondWith(Response.Create()
                    .WithStatusCode(200));
        }

        [TearDown]
        public void TearDown() =>
            _wiremock.Stop();

        [Test]
        public void ShouldMakeHttpRequestWhenSimpleHttpGet()
        {
            TestPlan(
                ThreadGroup(threads: 1, iterations: 1,
                    HttpSampler(_wiremock.Url)
                )
            ).Run();
            _wiremock.Should().HaveReceivedACall().UsingGet();
        }

        [Test]
        public void ShouldMakeHttpRequestWithBodyAndHeadersWhenHttpPost()
        {
            var customHeaderName = "X-Test";
            var customHeaderValue = "Val";
            var requestBody = "{\"prop\": \"val\"}";
            TestPlan(
                ThreadGroup(threads: 1, iterations: 1,
                    HttpSampler(_wiremock.Url)
                        .Post(requestBody, new MediaTypeHeaderValue(MediaTypeNames.Application.Json))
                        .Header(customHeaderName, customHeaderValue)
                )
            ).Run();
            _wiremock.Should()
                .HaveReceivedACall()
                .UsingPost()
                .And
                .WithHeader("Content-Type", "application/json")
                .And
                .WithHeader(customHeaderName, customHeaderValue)
                .And
                .WithBody(requestBody);
        }

        [Test]
        public void ShouldMakeHttpRequestWithHeaderWhenHeaderAtTestPlanLevel()
        {
            var customHeaderName = "X-Test";
            var customHeaderValue = "Val";
            TestPlan(
                HttpHeaders()
                    .Header(customHeaderName, customHeaderValue),
                ThreadGroup(threads: 1, iterations: 1,
                    HttpSampler(_wiremock.Url)
                )
            ).Run();
            _wiremock.Should()
                .HaveReceivedACall()
                .WithHeader(customHeaderName, customHeaderValue);
        }

        [Test]
        public void ShouldNotKeepCookiesWhenDisabled()
        {
            SetupHttpResponseWithCookie();
            TestPlan(
                HttpCookies().Disable(),
                ThreadGroup(1, 1,
                    HttpSampler(_wiremock.Url),
                    HttpSampler(_wiremock.Url)
                )
            ).Run();
            _wiremock.Should()
                .HaveReceivedACall()
                .WithoutHeader("Cookie");
        }

        private void SetupHttpResponseWithCookie()
        {
            _wiremock.Given(Request.Create().WithPath("/"))
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithHeader("Set-Cookie", "MyCookie=val"));
        }

        [Test]
        public void ShouldNotUseCacheWhenDisabled()
        {
            SetupCacheableHttpResponse();
            TestPlan(
                BuildHeadersToFixHttpCaching(),
                HttpCache().Disable(),
                ThreadGroup(1, 1,
                    HttpSampler(_wiremock.Url),
                    HttpSampler(_wiremock.Url)
                )
            ).Run();
            _wiremock.Should()
                .HaveReceived(2)
                .Calls()
                .UsingGet();
        }

        private void SetupCacheableHttpResponse()
        {
            _wiremock.Given(Request.Create().WithPath("/"))
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithHeader("Cache-Control", "max-age=600"));
        }

        /*
        need to set header for request header to match otherwise jmeter automatically adds this
        header while sending request and stores it in cache and when it checks in next request
        it doesn't match since same header is not yet set at check time.
        */
        private HttpHeaders BuildHeadersToFixHttpCaching() =>
            HttpHeaders().Header("User-Agent", "jmeter-java-dsl");
    }
}
