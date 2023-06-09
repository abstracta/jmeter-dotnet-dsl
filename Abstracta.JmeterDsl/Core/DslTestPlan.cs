using Abstracta.JmeterDsl.Core.Bridge;
using Abstracta.JmeterDsl.Core.Engines;
using Abstracta.JmeterDsl.Core.TestElements;

namespace Abstracta.JmeterDsl.Core
{
    /// <summary>
    /// Represents a JMeter test plan, with associated thread groups and other children elements.
    /// </summary>
    public class DslTestPlan : TestElementContainer<ITestPlanChild>
    {
        public DslTestPlan(ITestPlanChild[] children)
            : base(null, children)
        {
        }

        /// <summary>
        /// Uses <see cref="EmbeddedJmeterEngine"/> to run the test plan.
        /// </summary>
        /// <returns>A <see cref="TestPlanStats"/> object containing all statistics of the test plan execution.</returns>
        public TestPlanStats Run() =>
            new EmbeddedJmeterEngine().Run(this);

        /// <summary>
        /// Allows to run the test plan in a given engine.
        /// <br/>
        /// This method is just a simple method which provides fluent API to run the test plans in a given
        /// engine.
        /// </summary>
        /// <seealso cref="IDslJmeterEngine.Run(DslTestPlan)"/>
        public TestPlanStats RunIn(IDslJmeterEngine engine) =>
            engine.Run(this);

        /// <summary>
        /// Saves the given test plan as JMX, which allows it to be loaded in JMeter GUI.
        /// </summary>
        /// <param name="filePath">specifies where to store the JMX of the test plan.</param>
        public void SaveAsJmx(string filePath) =>
            new BridgeService().SaveTestPlanAsJmx(this, filePath);
    }
}
