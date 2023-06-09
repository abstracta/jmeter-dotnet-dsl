using System;
using Abstracta.JmeterDsl.Core.TestElements;
using Abstracta.JmeterDsl.Core.ThreadGroups;

namespace Abstracta.JmeterDsl.Core.Samplers
{
    /// <summary>
    /// Hosts common logic to all samplers.
    /// <br/>
    /// In particular, it specifies that samplers are <see cref="IThreadGroupChild"/> and <see cref="TestElementContainer{T}"/> containing <see cref="ISamplerChild"/>.
    /// For an example of an implementation of a sampler check <see cref="Http.DslHttpSampler"/>
    /// </summary>
    public abstract class BaseSampler : TestElementContainer<ISamplerChild>, IThreadGroupChild
    {
        protected BaseSampler(string name)
            : base(name, Array.Empty<ISamplerChild>())
        {
        }
    }
}
