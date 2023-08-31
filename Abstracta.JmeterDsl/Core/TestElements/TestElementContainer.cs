using System.Collections.Generic;
using System.Linq;

namespace Abstracta.JmeterDsl.Core.TestElements
{
    /// <summary>
    /// Abstracts logic for <see cref="IDslTestElement"/> that can nest other test elements.
    /// </summary>
    /// <typeparam name="T">is type of sub classes.</typeparam>
    /// <typeparam name="C">is type of test elements that can be nested by this class.</typeparam>
    public abstract class TestElementContainer<T, C> : BaseTestElement
        where T : TestElementContainer<T, C>
    {
        protected List<C> _children = new List<C>();

        protected TestElementContainer(string name, C[] children)
            : base(name)
        {
            _children = children.ToList();
        }

        protected T Children(params C[] chilren)
        {
            _children.AddRange(chilren);
            return (T)this;
        }
    }
}
