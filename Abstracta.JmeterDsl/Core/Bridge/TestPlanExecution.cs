namespace Abstracta.JmeterDsl.Core.Bridge
{
    public class TestPlanExecution
    {
        private readonly IDslJmeterEngine _engine;
        private readonly DslTestPlan _testPlan;

        public TestPlanExecution(IDslJmeterEngine engine, DslTestPlan testPlan)
        {
            _engine = engine;
            _testPlan = testPlan;
        }
    }
}
