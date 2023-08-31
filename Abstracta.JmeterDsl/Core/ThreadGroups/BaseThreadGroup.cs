using Abstracta.JmeterDsl.Core.TestElements;

namespace Abstracta.JmeterDsl.Core.ThreadGroups
{
    /// <summary>
    /// Contains common logic for all Thread Groups.
    /// </summary>
    public abstract class BaseThreadGroup<T> : TestElementContainer<T, IThreadGroupChild>, ITestPlanChild
        where T : BaseThreadGroup<T>
    {
        protected BaseThreadGroup(string name, IThreadGroupChild[] children)
            : base(name, children)
        {
        }
    }
}
