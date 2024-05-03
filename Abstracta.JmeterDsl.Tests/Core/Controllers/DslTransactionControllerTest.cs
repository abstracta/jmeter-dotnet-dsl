namespace Abstracta.JmeterDsl.Core.Controllers
{
    using static JmeterDsl;

    public class DslTransactionControllerTest
    {
        [Test]
        public void ShouldIncludeTransactionSampleInResultsWhenTestPlanWithTransaction()
        {
            TestPlanStats stats = TestPlan(
                ThreadGroup(1, 1,
                    Transaction("My Transaction",
                        DummySampler("ok")
                    )
                )
            ).Run();
            Assert.That(stats.Overall.SamplesCount, Is.EqualTo(2));
        }
    }
}
