namespace Abstracta.JmeterDsl.Core.PreProcessors
{
    using static JmeterDsl;

    public class DslJsr223PreProcessorTest
    {
        [Test]
        public void ShouldUseDefinedLabelWhenPreProcessorSettingLabelInTestPlan()
        {
            var stats = TestPlan(
                    ThreadGroup(1, 1,
                        DummySampler("ok")
                            .Children(
                                Jsr223PreProcessor("sampler.name = 'test'")
                            )
                    )).Run();
            Assert.That(stats.Labels["test"].SamplesCount, Is.EqualTo(1));
        }
    }
}
