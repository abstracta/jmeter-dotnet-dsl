using Abstracta.JmeterDsl.Core.Configs;

namespace Abstracta.JmeterDsl.Http
{
    /// <summary>
    /// Allows configuring cookies settings used by HTTP samplers.
    /// <br/>
    /// This element can only be added as child of test plan, and currently allows only to disable HTTP
    /// cookies handling which is enabled by default (emulating browser behavior).
    /// <br/>
    /// This element has to be added before any http sampler to be considered, and if you add multiple
    /// instances of cookie manager to a test plan, only the first one will be considered.
    /// </summary>
    public class DslHttpCookies : BaseConfigElement
    {
        protected bool? _disable;
        protected bool? _clearCookiesBetweenIterations;

        public DslHttpCookies()
            : base(null)
        {
        }

        /// <summary>
        /// Disables HTTP cookies handling for the test plan.
        /// </summary>
        /// <returns>the DslHttpCookies to allow fluent API usage.</returns>
        public DslHttpCookies Disable()
        {
            _disable = true;
            return this;
        }

        /// <summary>
        /// Allows to enable or disable clearing cookies between thread group iterations.
        /// <br/>
        /// Cookies are cleared each iteration by default. If this is not desirable, for instance if
        /// logging in once and then iterating through actions multiple times, use this to set to false.
        /// </summary>
        /// <param name="clear">clear boolean to set clearing of cookies. By default, it is set to true.</param>
        /// <returns>the DslHttpCookies for further configuration or usage.</returns>
        public DslHttpCookies ClearCookiesBetweenIterations(bool clear)
        {
            _clearCookiesBetweenIterations = clear;
            return this;
        }
    }
}
