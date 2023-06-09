namespace Abstracta.JmeterDsl.Core
{
    /// <summary>
    /// Test elements that can be added directly as test plan children in JMeter should implement this interface.
    /// Check <see cref="ThreadGroups.DslThreadGroup"/> for an example.
    /// </summary>
    public interface ITestPlanChild : IDslTestElement
    {
    }
}
