using WireMock.FluentAssertions;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace Abstracta.JmeterDsl.Core.PostProcessors
{
    using static JmeterDsl;

    public class DslRegexExtractorTest
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
        public void ShouldExtractVariableWhenSimpleRegexExtractorMatches()
        {
            var user = "test";
            var userParam = "user=";
            var userVar = "USER";
            TestPlan(
                    ThreadGroup(1, 1,
                        DummySampler(userParam + user)
                            .Children(
                                RegexExtractor(userVar, userParam + "(.*)")
                            ),
                        HttpSampler(_wiremock.Url + "/?" + userParam + "${" + userVar + "}")
                    )).Run();
            _wiremock.Should().HaveReceivedACall().AtUrl(_wiremock.Url + "/?" + userParam + user);
        }

        [Test]
        public void ShouldExtractVariableWhenComplexRegexExtractorMatches()
        {
            var user = "test";
            var userParam = "user=";
            var userVar = "USER";
            TestPlan(
                    ThreadGroup(1, 1,
                        DummySampler("OK")
                        .Url("http://localhost/?" + userParam + "user2&" + userParam + user)
                            .Children(
                                RegexExtractor(userVar, "([^&?]+)=([^&]+)")
                                .MatchNumber(2)
                                .Template("$2$")
                                .FieldToCheck(DslRegexExtractor.DslTargetField.RequestUrl)
                                .Scope(TestElements.DslScopedTestElement<DslRegexExtractor>.DslScope.AllSamples)
                            ),
                        HttpSampler(_wiremock.Url + "/?" + userParam + "${" + userVar + "}")
                    )).Run();
            _wiremock.Should().HaveReceivedACall().AtUrl(_wiremock.Url + "/?" + userParam + user);
        }
    }
}