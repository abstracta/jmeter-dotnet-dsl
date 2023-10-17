using Abstracta.JmeterDsl.Core.Bridge;

namespace Abstracta.JmeterDsl.Core.TestElements
{
    /// <summary>
    /// Provides the basic logic for all <see cref="IDslTestElement"/>.
    /// </summary>
    public abstract class BaseTestElement : IDslTestElement
    {
        protected readonly string _name;

        public BaseTestElement(string name)
        {
            _name = name;
        }

        public void ShowInGui() =>
            new BridgeService().ShowTestElementInGui(this);
    }
}
