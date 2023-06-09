using Abstracta.JmeterDsl.Core.TestElements;

namespace Abstracta.JmeterDsl.Core.ThreadGroups
{
    /// <summary>
    /// Contains common logic for all Thread Groups.
    /// </summary>
    public abstract class BaseThreadGroup : TestElementContainer<IThreadGroupChild>, ITestPlanChild
    {
        protected BaseThreadGroup(string name, IThreadGroupChild[] children)
            : base(name, children)
        {
        }
    }
}
