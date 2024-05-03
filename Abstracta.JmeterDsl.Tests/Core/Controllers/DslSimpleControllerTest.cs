namespace Abstracta.JmeterDsl.Core.Controllers
{
    using static JmeterDsl;

    public class DslSimpleControllerTest
    {
        [Test]
        public void ShouldApplyAssertionToSimpleControllerScopedElements()
        {
            var body = "FAIL";
            var stats = TestPlan(
                ThreadGroup(1, 1,
                    SimpleController(
                        ResponseAssertion()
                            .ContainsSubstrings("OK"),
                        DummySampler(body),
                        DummySampler(body)
                    ),
                    DummySampler(body)
                )
            ).Run();
            Assert.That(stats.Overall.ErrorsCount, Is.EqualTo(2));
        }
    }
}
