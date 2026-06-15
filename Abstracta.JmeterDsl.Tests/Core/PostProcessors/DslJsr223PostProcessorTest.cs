namespace Abstracta.JmeterDsl.Core.PostProcessors
{
    using static JmeterDsl;

    public class DslJsr223PostProcessorTest
    {
        [Test]
        public void ShouldReportNoFailureWhenJsr223PostProcessorModifiesFailedRequest()
        {
            var stats = TestPlan(
                    ThreadGroup(1, 1,
                        HttpSampler("invalidUrl")
                            .Children(
                                Jsr223PostProcessor("prev.setSuccessful(true)")
                            )
                    )).Run();
            Assert.That(stats.Overall.ErrorsCount, Is.EqualTo(0));
        }
    }
}
