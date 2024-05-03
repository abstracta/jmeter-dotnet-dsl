using Abstracta.JmeterDsl.Core.TestElements;

namespace Abstracta.JmeterDsl.Core.Timers
{
    /// <summary>
    /// Contains common logic for all timers.
    /// </summary>
    public abstract class BaseTimer : BaseTestElement, IMultiLevelTestElement
    {
        public BaseTimer(string name)
            : base(name)
        {
        }
    }
}
