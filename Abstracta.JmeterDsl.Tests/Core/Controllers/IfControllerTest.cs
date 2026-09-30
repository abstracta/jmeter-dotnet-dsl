namespace Abstracta.JmeterDsl.Core.Controllers
{
    using static JmeterDsl;

    public class IfControllerTest
    {
        [Test]
        public void ShouldExecuteChildrenWhenConditionIsTrue()
        {
            var stats = TestPlan(
                ThreadGroup(1, 1,
                    IfController(
                        "${__groovy(true)}",
                        DummySampler("ok")
                    )
                )).Run();
            Assert.That(stats.Overall.SamplesCount, Is.EqualTo(1));
        }

        [Test]
        public void ShouldNotExecuteChildrenWhenConditionIsFalse()
        {
            var stats = TestPlan(
                ThreadGroup(1, 1,
                    DummySampler("alwaysRun"),
                    IfController(
                        "${__groovy(false)}",
                        DummySampler("ok")
                    )
                )).Run();
            Assert.Multiple(() =>
            {
                Assert.That(stats.Overall.SamplesCount, Is.EqualTo(1));
                Assert.That(stats.Labels.ContainsKey("ok"), Is.False);
            });
        }
    }
}
