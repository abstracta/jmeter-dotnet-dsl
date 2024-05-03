using Abstracta.JmeterDsl.Core.ThreadGroups;

namespace Abstracta.JmeterDsl.Core.Controllers
{
    /// <summary>
    /// Builds a Simple Controller that allows defining new JMeter scope for other elements to apply.
    /// <br/>
    /// This is handy for example to apply timers, configs, listeners, assertions, pre- and
    /// post-processors to only some samplers in the test plan.
    /// <br/>
    /// It has a similar functionality as the transaction controller, but it doesn't add any additional
    /// sample results (statistics) to the test plan.
    /// </summary>
    public class DslSimpleController : BaseController<DslSimpleController>
    {
        public DslSimpleController(string name, IThreadGroupChild[] children)
          : base(name, children)
        {
        }
    }
}
