using Abstracta.JmeterDsl.Core.ThreadGroups;

namespace Abstracta.JmeterDsl.Core.Controllers
{
    /// <summary>
    /// Allows running part of a test plan until a condition is met.
    /// <br/>
    /// The condition is evaluated in each iteration before and after all children elements are executed.
    /// Keep this in mind in case you use conditions with side effects (like incrementing counters).
    /// <br/>
    /// JMeter automatically creates a variable named <c>__jm__&lt;controllerName&gt;__idx</c> which contains
    /// the index of the iteration starting with zero.
    /// </summary>
    public class WhileController : BaseController<WhileController>
    {
        private readonly string _condition;

        public WhileController(string name, string condition, IThreadGroupChild[] children)
          : base(name, children)
        {
            _condition = condition;
        }
    }
}
