using Abstracta.JmeterDsl.Core.ThreadGroups;

namespace Abstracta.JmeterDsl.Core.Controllers
{
    /// <summary>
    /// Allows running part of a test plan only if a condition is met.
    /// <br/>
    /// Internally this uses JMeter If Controller.
    /// <br/>
    /// The condition is evaluated by JMeter using JMeter expressions (e.g.: <c>${__groovy(...)}</c>)
    /// which must evaluate to <c>true</c> or <c>false</c> to determine whether children elements are executed.
    /// </summary>
    public class IfController : BaseController<IfController>
    {
        private readonly string _condition;

        public IfController(string name, string condition, IThreadGroupChild[] children)
          : base(name, children)
        {
            _condition = condition;
        }
    }
}
