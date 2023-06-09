using Abstracta.JmeterDsl.Core.TestElements;

namespace Abstracta.JmeterDsl.Core.Configs
{
    /// <summary>
    /// Contains common logic for config elements defined by the DSL.
    /// </summary>
    public abstract class BaseConfigElement : BaseTestElement, IMultiLevelTestElement
    {
        public BaseConfigElement(string name)
            : base(name)
        {
        }
    }
}
