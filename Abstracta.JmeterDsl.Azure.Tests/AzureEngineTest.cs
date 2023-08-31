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
        [Ignore("This azure test is now reporting 0 samples counts, even though we get the sample "
            + "result in results file but general dashboard in Azure is reporting 0 as well. "
            + "We will disable it until we get an answer from Azure Load Testing support.")]
        public void TestInAzure()
        {
            var stats = TestPlan(
                ThreadGroup(1, 1,
                    HttpSampler("http://localhost")
                )
            ).RunIn(new AzureEngine(Environment.GetEnvironmentVariable("AZURE_CREDS")));
            Assert.That(stats.Overall.ErrorsCount, Is.EqualTo(1));
        }
    }
}
