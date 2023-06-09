using Abstracta.JmeterDsl.Core.Configs;

namespace Abstracta.JmeterDsl.Http
{
    /// <summary>
    /// Allows configuring caching behavior used by HTTP samplers.
    /// <br/>
    /// This element can only be added as child of test plan, and currently allows only to disable HTTP
    /// caching which is enabled by default (emulating browser behavior).
    /// <br/>
    /// This element has to be added before any http sampler to be considered, and if you add multiple
    /// instances of cache manager to a test plan, only the first one will be considered.
    /// </summary>
    public class DslHttpCache : BaseConfigElement
    {
        protected bool? _disable;

        public DslHttpCache()
            : base(null)
        {
        }

        /// <summary>
        /// disables HTTP caching for the test plan.
        /// </summary>
        /// <returns>the DslHttpCache to allow fluent API usage.</returns>
        public DslHttpCache Disable()
        {
            _disable = true;
            return this;
        }
    }
}
