using System;

namespace Abstracta.JmeterDsl.Core.ThreadGroups
{
    using static JmeterDsl;

    public class DslThreadGroupTest
    {
        [Test]
        public void ShouldMakeOneRequestWhenOneThreadAndIteration()
        {
            var stats = TestPlan(
                ThreadGroup(threads: 1, iterations: 1,
                    DummySampler("OK")
                )
            ).Run();
            Assert.That(stats.Overall.SamplesCount, Is.EqualTo(1));
        }

        [Test]
        public void ShouldTakeAtLeastDurationWhenThreadGroupWithDuration()
        {
            var duration = TimeSpan.FromSeconds(5);
            var stats = TestPlan(
                ThreadGroup(threads: 1, duration: duration,
                    DummySampler("OK")
                )
            ).Run();
            Assert.That(stats.Duration, Is.GreaterThanOrEqualTo(duration));
        }

        [Test]
        public void ShouldStartNextIterationWhenSamplerFailsAndStartNextIterationActionConfigured()
        {
            var stats = TestPlan(
                ThreadGroup(threads: 1, iterations: 2,
                    DummySampler("first")
                        .Successful(false),
                    DummySampler("second")
                ).SampleErrorAction(BaseThreadGroup<DslThreadGroup>.DslSampleErrorAction.StartNextIteration)
            ).Run();
            Assert.Multiple(() =>
            {
                Assert.That(stats.Overall.SamplesCount, Is.EqualTo(2));
                Assert.That(stats.Overall.ErrorsCount, Is.EqualTo(2));
                Assert.That(stats.Labels.ContainsKey("second"), Is.False);
            });
        }
    }
}
