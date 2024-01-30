using Abstracta.JmeterDsl.Core.TestElements;
using Abstracta.JmeterDsl.Core.ThreadGroups;

namespace Abstracta.JmeterDsl.Core.Controllers
{
    /// <summary>
    /// Contains common logic for logic controllers defined by the DSL.
    /// </summary>
    public abstract class BaseController<T> : TestElementContainer<T, IThreadGroupChild>, IThreadGroupChild
        where T : BaseController<T>
    {
        protected BaseController(string name, IThreadGroupChild[] children)
          : base(name, children)
        {
        }

        /// <summary>
        /// Allows specifying children elements that are affected by this controller.
        /// <br/>
        /// This method is helpful to keep general controller settings at the beginning and specify
        /// children at last.
        /// </summary>
        /// <param name="children">set of elements to be included in the controller. This list is appended to any children defined in controller builder method.</param>
        /// <returns>a new controller instance for further configuration or usage.</returns>
        public new T Children(params IThreadGroupChild[] children)
          => base.Children(children);
    }
}
