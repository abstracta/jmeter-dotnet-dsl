namespace Abstracta.JmeterDsl.Core.Controllers
{
    using static JmeterDsl;

    public class ForLoopControllerTest
    {
        [Test]
        public void ShouldGetExpectedSamplerCountWhenSamplerInsideLoopController()
        {
            var loopIterations = 3;
            var stats = TestPlan(
                ThreadGroup(1, 1,
                    ForLoopController(loopIterations,
                        DummySampler("ok")
                    )
                )).Run();
            Assert.That(stats.Overall.SamplesCount, Is.EqualTo(loopIterations));
        }
    }
}
