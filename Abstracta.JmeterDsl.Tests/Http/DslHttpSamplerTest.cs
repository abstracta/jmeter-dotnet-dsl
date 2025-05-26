using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using WireMock.FluentAssertions;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace Abstracta.JmeterDsl.Http
{
    using static JmeterDsl;

    public class DslHttpSamplerTest
    {
        private static readonly string _contentTypeHeader = "Content-Type";
        private static readonly string _multipartBoundaryPattern = "[\\w-]+";
        private static readonly string _crln = "\r\n";
        private string _redirectPath = "/redirect";
        private string _finalDestinationPath = "/final-destination";
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
                .WithHeader(_contentTypeHeader, "application/json")
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

        [Test]
        public void ShouldSendQueryParametersWhenGetRequestWithParameters()
        {
            var param1Name = "par+am1";
            var param1Value = "MY+VALUE";
            var param2Name = "par+am2";
            var param2Value = "OTHER+VALUE";
            TestPlan(
                    ThreadGroup(1, 1,
                        HttpSampler(_wiremock.Url)
                            .Param(param1Name, param1Value)
                            .RawParam(param2Name, param2Value)
                    )
                ).Run();
            _wiremock.Should()
                .HaveReceivedACall()
                .AtUrl(_wiremock.Url + "/?" + HttpUtility.UrlEncode(param1Name) + "=" + HttpUtility.UrlEncode(param1Value) + "&" + param2Name + "=" + param2Value);
        }

        [Test]
        public void ShouldSendMultiPartFormWhenPostRequestWithBodyParts()
        {
            var part1Name = "part1";
            var part1Value = "value1";
            var part1Encoding = MediaTypeHeaderValue.Parse(MediaTypeNames.Text.Plain + "; charset=US-ASCII");
            var part2Name = "part2";
            var part2File = "Http/sample.xml";
            var part2Encoding = new MediaTypeHeaderValue(MediaTypeNames.Text.Xml);

            TestPlan(
                ThreadGroup(1, 1,
                    HttpSampler(_wiremock.Url)
                        .Method(HttpMethod.Post.Method)
                        .BodyPart(part1Name, part1Value, part1Encoding)
                        .BodyFilePart(part2Name, part2File, part2Encoding)
                )
            ).Run();
            _wiremock.Should()
                .HaveReceivedACall()
                .UsingPost()
                .And
                .WithHeader(_contentTypeHeader, new Regex("multipart/form-data; boundary=" + _multipartBoundaryPattern))
                .And
                .WithBody(new Regex(BuildMultiPartBodyPattern(part1Name, part1Value, part1Encoding, part2Name, part2File, part2Encoding)));
        }

        private void SetupHttpResponseRedirect()
        {
            _wiremock.Given(Request.Create().WithPath(_redirectPath))
                .RespondWith(Response.Create()
                    .WithStatusCode(302)
                    .WithHeader("Location", _finalDestinationPath));
        }

        [Test]
        public void ShouldFollowRedirectsByDefault()
        {
            SetupHttpResponseRedirect();

            TestPlan(
                ThreadGroup(1, 1,
                    HttpSampler(_wiremock.Url + _redirectPath).
                        Method(HttpMethod.Get.Method)
                )
            ).Run();

            _wiremock.Should()
                .HaveReceivedACall()
                .WithPath(_redirectPath);

            _wiremock.Should()
                .HaveReceivedACall()
                .WithPath(_finalDestinationPath);
        }

        [Test]
        public void ShouldNotFollowRedirectsByDefault()
        {
            SetupHttpResponseRedirect();

            TestPlan(
                ThreadGroup(1, 1,
                    HttpSampler(_wiremock.Url + _redirectPath).
                        Method(HttpMethod.Get.Method)
                        .FollowRedirects(false)
                )
            ).Run();

            _wiremock.Should()
                .HaveReceivedACall()
                .WithPath(_redirectPath);

            var finalDestinationCallCount = _wiremock.LogEntries.Count(x => x.RequestMessage.Path == _finalDestinationPath);
            Assert.That(finalDestinationCallCount, Is.EqualTo(0), $"An unexpected request was received at {_finalDestinationPath}.");
        }

        private string BuildMultiPartBodyPattern(string part1Name, string part1Value, MediaTypeHeaderValue part1Encoding, string part2Name, string part2File, MediaTypeHeaderValue part2Encoding)
        {
            var separatorPattern = "--" + _multipartBoundaryPattern;
            return separatorPattern + _crln
                + Regex.Escape(BuildBodyPart(part1Name, null, part1Value, part1Encoding, "8bit"))
                + separatorPattern + _crln
                + Regex.Escape(BuildBodyPart(part2Name, Path.GetFileName(part2File), File.ReadAllText(part2File), part2Encoding, "binary"))
                + separatorPattern + "--" + _crln;
        }

        private string BuildBodyPart(string name, string fileName, string value, MediaTypeHeaderValue contentType, string transferEncoding)
        {
            return "Content-Disposition: form-data; name=\"" + name + "\""
                + (fileName != null ? "; filename=\"" + fileName + "\"" : string.Empty) + _crln
                + "Content-Type" + ": " + contentType + _crln
                + "Content-Transfer-Encoding: " + transferEncoding + _crln
                + _crln
                + value + _crln;
        }
    }
}
