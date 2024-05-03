namespace Abstracta.JmeterDsl.Core.Timers
{
    using System;
    using static JmeterDsl;

    public class DslUniformRandomTimerTest
    {
        [Test]
        public void ShouldLastAtLeastMinimumTimeWhenUsingRandomUniformTimer()
        {
            var minimum = TimeSpan.FromSeconds(5);
            var stats = TestPlan(
                ThreadGroup(1, 1,
                    UniformRandomTimer(minimum, TimeSpan.FromSeconds(7)),
                    DummySampler("OK")
                )
            ).Run();
            Assert.That(stats.Duration, Is.GreaterThan(minimum));
        }
    }
}
