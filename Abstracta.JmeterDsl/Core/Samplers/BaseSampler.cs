using System;
using Abstracta.JmeterDsl.Core.TestElements;
using Abstracta.JmeterDsl.Core.ThreadGroups;

namespace Abstracta.JmeterDsl.Core.Samplers
{
    /// <summary>
    /// Hosts common logic to all samplers.
    /// <br/>
    /// In particular, it specifies that samplers are <see cref="IThreadGroupChild"/> and <see cref="TestElementContainer{T, C}"/> containing <see cref="ISamplerChild"/>.
    /// For an example of an implementation of a sampler check <see cref="Http.DslHttpSampler"/>
    /// </summary>
    public abstract class BaseSampler<T> : TestElementContainer<T, ISamplerChild>, IThreadGroupChild
        where T : BaseSampler<T>
    {
        protected BaseSampler(string name)
            : base(name, Array.Empty<ISamplerChild>())
        {
        }

        /// <summary>
        /// Allows specifying children test elements for the sampler, which allow for example extracting
        /// information from response, alter request, assert response contents, etc.
        /// </summary>
        /// <param name="children">list of test elements to add as children of this sampler.</param>
        /// <returns>the altered sampler to allow for fluent API usage.</returns>
        public new T Children(params ISamplerChild[] children)
            => base.Children(children);
    }
}
