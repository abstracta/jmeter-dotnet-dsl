namespace Abstracta.JmeterDsl.Core.Timers
{
    using System;
    using static JmeterDsl;

    public class DslConstantTimerTest
    {
        [Test]
        public void ShouldLastAtLeastConfiguredTimeWhenUsingConstantTimer()
        {
            var timerDuration = TimeSpan.FromSeconds(5);
            var stats = TestPlan(
                ThreadGroup(1, 1,
                    ConstantTimer(timerDuration),
                    DummySampler("OK")
                )
            ).Run();
            Assert.That(stats.Duration, Is.GreaterThan(timerDuration));
        }
    }
}
