using Abstracta.JmeterDsl.Core.TestElements;

namespace Abstracta.JmeterDsl.Core.PreProcessors
{
    /// <summary>
    /// Allows running custom logic before executing a sampler.
    /// <br/>
    /// This is a very powerful and flexible component that allows you to modify variables, sampler,
    /// context, etc., before running a sampler (for example to generate dynamic requests
    /// programmatically).
    /// <br/>
    /// By default, provided script will be interpreted as groovy script, which is the default setting
    /// for JMeter. If you need, you can use any of JMeter provided scripting languages (beanshell,
    /// javascript, jexl, etc.) by setting the <see cref="DslJsr223TestElement{T}.Language(string)"/> property.
    /// </summary>
    public class DslJsr223PreProcessor : DslJsr223TestElement<DslJsr223PreProcessor>, IMultiLevelTestElement
    {
        public DslJsr223PreProcessor(string name, string script)
            : base(name, script)
        {
        }
    }
}