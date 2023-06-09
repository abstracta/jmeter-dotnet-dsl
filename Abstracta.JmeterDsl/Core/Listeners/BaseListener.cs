using Abstracta.JmeterDsl.Core.TestElements;

namespace Abstracta.JmeterDsl.Core.Listeners
{
    public abstract class BaseListener : BaseTestElement, IMultiLevelTestElement
    {
        protected BaseListener()
            : base(null)
        {
        }
    }
}
