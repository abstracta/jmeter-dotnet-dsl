using System;
using System.IO;

namespace Abstracta.JmeterDsl.Azure.Tests
{
    using static JmeterDsl;

    public class AzureEngineTest
    {

        private TextWriter originalConsoleOut;

        // Redirecting output to progress to get live stdout with nunit.
        // https://github.com/nunit/nunit3-vs-adapter/issues/343
        // https://github.com/nunit/nunit/issues/1139
        [SetUp]
        public void SetUp()
        {
            originalConsoleOut = Console.Out;
            Console.SetOut(TestContext.Progress);
        }

        [TearDown]
        public void TearDown() =>
            Console.SetOut(originalConsoleOut!);

        [Test]
        public void TestInAzure()
        {
            var stats = TestPlan(
                ThreadGroup(1, 1,
                    HttpSampler("http://localhost")
                )
            ).RunIn(new AzureEngine(Environment.GetEnvironmentVariable("AZURE_CREDS")));
            Assert.That(stats.Overall.ErrorsCount, Is.EqualTo(1));
        }

        [Test]
        public void ShouldUseDifferentCsvRecordsInEachEngineWhenSplitCsvsBetweenEngines()
        {
            var stats = TestPlan(
                CsvDataSet("users.csv")
                    .VariableNames("USER"),
                ThreadGroup(1, 1,
                    DummySampler("${USER}", "ok")
                )
            ).RunIn(new AzureEngine(Environment.GetEnvironmentVariable("AZURE_CREDS"))
                .Engines(2)
                .SplitCsvsBetweenEngines(true));
            Assert.Multiple(() =>
            {
                Assert.That(stats.Labels["user1"].SamplesCount, Is.EqualTo(1));
                Assert.That(stats.Labels["user2"].SamplesCount, Is.EqualTo(1));
            });
        }
    }
}
