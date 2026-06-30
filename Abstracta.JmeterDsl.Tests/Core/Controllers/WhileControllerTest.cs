namespace Abstracta.JmeterDsl.Core.Controllers
{
    using static JmeterDsl;

    public class WhileControllerTest
    {
        private const string CallsVar = "CALLS";
        private const int StopWhenCallsReaches = 3;

        [Test]
        public void ShouldExecuteChildrenWhileConditionIsTrue()
        {
            var stats = TestPlan(
                ThreadGroup(1, 1,
                    WhileController(
                        "${__groovy(vars.getObject('" + CallsVar + "') != " + StopWhenCallsReaches + ")}",
                        DummySampler("ok")
                            .Children(
                                Jsr223PreProcessor(
                                    "def prevCalls = vars.getObject('" + CallsVar + "')\n" +
                                    "vars.putObject('" + CallsVar + "', prevCalls == null ? 1 : prevCalls + 1)")
                            )
                    )
                )).Run();
            Assert.That(stats.Overall.SamplesCount, Is.EqualTo(StopWhenCallsReaches));
        }
    }
}
