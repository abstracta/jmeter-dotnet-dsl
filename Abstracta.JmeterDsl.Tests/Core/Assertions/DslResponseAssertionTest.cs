namespace Abstracta.JmeterDsl.Core.Assertions
{
    using static JmeterDsl;

    public class DslResponseAssertionTest
    {
        [Test]
        public void ShouldNotFailAssertionWhenResponseAssertionWithMatchingCondition()
        {
            var stats = TestPlan(
                    ThreadGroup(1, 1,
                        DummySampler("OK")
                            .Children(
                                ResponseAssertion().ContainsSubstrings("OK")
                            )
                    )).Run();
            Assert.That(stats.Overall.ErrorsCount, Is.EqualTo(0));
        }

        [Test]
        public void ShouldFailAssertionWhenResponseAssertionWithNotMatchingCondition()
        {
            var stats = TestPlan(
                    ThreadGroup(1, 1,
                        DummySampler("OK")
                            .Children(
                                ResponseAssertion().ContainsSubstrings("FAIL")
                            )
                    )).Run();
            Assert.That(stats.Overall.ErrorsCount, Is.EqualTo(1));
        }
    }
}