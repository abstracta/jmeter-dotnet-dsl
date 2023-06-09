using Abstracta.JmeterDsl.Core.Samplers;
using Abstracta.JmeterDsl.Core.ThreadGroups;

namespace Abstracta.JmeterDsl.Core.TestElements
{
    /// <summary>
    /// This is just a simple interface to avoid code duplication for test elements that apply at
    /// different levels of a test plan(at test plan, thread group or as sampler child).
    /// </summary>
    public interface IMultiLevelTestElement : ITestPlanChild, IThreadGroupChild, ISamplerChild
    {
    }
}
