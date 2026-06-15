using WireMock.FluentAssertions;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace Abstracta.JmeterDsl.Core.PostProcessors
{
    using static JmeterDsl;

    public class DslJsonExtractorTest
    {
        private WireMockServer _wiremock;

        [SetUp]
        public void SetUp() => _wiremock = WireMockServer.Start();

        [TearDown]
        public void TearDown() =>
            _wiremock.Stop();

        [Test]
        public void ShouldExtractVariableWhenJmesPathJsonExtractorMatchesResponse() => TestJsonExtractor(JsonExtractor("EXTRACTED_USER", "[].name"));

        [Test]
        public void ShouldExtractVariableWhenJsonPathJsonExtractorMatchesResponse()
        {
            TestJsonExtractor(JsonExtractor("EXTRACTED_USER", "$[*].name")
                .QueryLanguage(DslJsonExtractor.DslJsonQueryLanguage.JsonPath));
        }

        private void TestJsonExtractor(DslJsonExtractor extractor)
        {
            var path = "/users";
            var user = "test";
            _wiremock.Given(Request.Create().WithPath(path))
                .RespondWith(Response.Create()
                    .WithBody($"[{{\"name\":\"{user}\"}}]"));
            var userQueryParameter = "?name=";
            TestPlan(
                    ThreadGroup(1, 1,
                        HttpSampler(_wiremock.Url + path)
                            .Children(extractor),
                        HttpSampler(_wiremock.Url + path + userQueryParameter + "${EXTRACTED_USER}")
                    )).ShowInGui();
            _wiremock.Should().HaveReceivedACall()
                .AtUrl(_wiremock.Url + path + userQueryParameter + user);
        }
    }
}
