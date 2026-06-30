using Abstracta.JmeterDsl.Core.TestElements;

namespace Abstracta.JmeterDsl.Core.PostProcessors
{
    /// <summary>
    /// Allows running custom logic after getting a sample result.
    /// <br/>
    /// This is a very powerful and flexible component that allows you to modify sample results (like
    /// changing the flag if is success or not), jmeter variables, context settings, etc.
    /// <br/>
    /// By default, provided script will be interpreted as groovy script, which is the default setting
    /// for JMeter. If you need, you can use any of JMeter provided scripting languages (beanshell,
    /// javascript, jexl, etc.) by setting the <see cref="DslJsr223TestElement{T}.Language(string)"/> property.
    /// </summary>
    public class DslJsr223PostProcessor : DslJsr223TestElement<DslJsr223PostProcessor>, IMultiLevelTestElement
    {
        public DslJsr223PostProcessor(string name, string script)
            : base(name, script)
        {
        }
    }
}
