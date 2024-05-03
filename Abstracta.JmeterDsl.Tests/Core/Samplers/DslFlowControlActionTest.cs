using System;

namespace Abstracta.JmeterDsl.Core.Samplers
{
    using static JmeterDsl;

    public class DslFlowControlActionTest
    {
        [Test]
        public void ShouldLastAtLeastConfiguredTimeWhenUsingConstantTimer()
        {
            var timerDuration = TimeSpan.FromSeconds(5);
            var stats = TestPlan(
                ThreadGroup(1, 1,
                    ThreadPause(timerDuration),
                    DummySampler("OK")
                )
            ).Run();
            Assert.That(stats.Duration, Is.GreaterThan(timerDuration));
        }
    }
}
