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
    }
}
