namespace Abstracta.JmeterDsl.Core
{
    /// <summary>
    /// Interface to be implemented by all elements composing a JMeter test plan.
    /// </summary>
    public interface IDslTestElement
    {
        /// <summary>
        /// Shows the test element in it's defined GUI in a popup window.
        /// <br/>
        /// This might be handy to visualize the element as it looks in JMeter GUI.
        /// </summary>
        void ShowInGui();
    }
}
