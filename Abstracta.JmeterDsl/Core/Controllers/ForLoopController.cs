using Abstracta.JmeterDsl.Core.ThreadGroups;

namespace Abstracta.JmeterDsl.Core.Controllers
{
    /// <summary>
    /// Allows running part of a test plan a given number of times inside one thread group iteration.
    /// <br/>
    /// Internally this uses JMeter Loop Controller.
    /// <br/>
    /// JMeter automatically creates a variable named <c>__jm__&lt;controllerName&gt;__idx</c> which contains
    /// the index of the iteration starting with zero.
    /// </summary>
    public class ForLoopController : BaseController<ForLoopController>
    {
        private readonly string _count;

        public ForLoopController(string name, string count, IThreadGroupChild[] children)
          : base(name, children)
        {
            _count = count;
        }
    }
}