namespace Abstracta.JmeterDsl.Core
{
    /// <summary>
    /// Interface to be implemented by classes allowing to run a DslTestPlan in different engines.
    /// </summary>
    public interface IDslJmeterEngine
    {
        /// <summary>
        /// Runs the given test plan obtaining the execution metrics.
        /// <br/>
        /// This method blocks execution until the test plan execution ends.
        /// </summary>
        /// <param name="testPlan">to run in the JMeter engine.</param>
        /// <returns>the metrics associated to the run.</returns>
        TestPlanStats Run(DslTestPlan testPlan);
    }
}
