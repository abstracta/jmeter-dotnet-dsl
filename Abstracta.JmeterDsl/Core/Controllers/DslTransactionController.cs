using Abstracta.JmeterDsl.Core.Bridge;
using Abstracta.JmeterDsl.Core.ThreadGroups;

namespace Abstracta.JmeterDsl.Core.Controllers
{
    /// <summary>
    /// Allows specifying JMeter transaction controllers which group different samples associated to same
    /// transaction.
    /// <br/>
    /// This is usually used when grouping different steps of a flow, for example group requests of login
    /// flow, adding item to cart, purchase, etc. It provides aggregate metrics of all it's samples.
    /// </summary>
    [YamlType(TagName = "transaction")]
    public class DslTransactionController : BaseController<DslTransactionController>
    {
        private bool _includeTimersAndProcessorsTime;
        private bool _generateParentSample;

        public DslTransactionController(string name, IThreadGroupChild[] children)
          : base(name, children)
        {
        }

        /// <summary>
        /// Specifies to include time spent in timers and pre- and post-processors in sample results.
        /// </summary>
        /// <returns>the controller for further configuration or usage.</returns>
        public DslTransactionController IncludeTimersAndProcessorsTime() =>
            IncludeTimersAndProcessorsTime(true);

        /// <summary>
        /// Same as <see cref="IncludeTimersAndProcessorsTime()"/> but allowing to enable or disable it.
        /// <br/>
        /// This is helpful when the resolution is taken at runtime.
        /// </summary>
        /// <param name="enable">specifies to enable or disable the setting. By default, it is set to false.</param>
        /// <returns>the controller for further configuration or usage.</returns>
        /// <seealso cref="IncludeTimersAndProcessorsTime()"/>
        public DslTransactionController IncludeTimersAndProcessorsTime(bool enable)
        {
            _includeTimersAndProcessorsTime = enable;
            return this;
        }

        /// <summary>
        /// Specifies to create a sample result as parent of children samplers.
        /// <br/>
        /// It is useful in some scenarios to get transaction sample results as parent of children samplers
        /// to focus mainly in transactions and not be concerned about each particular request. Enabling
        /// parent sampler helps in this regard, only reporting the transactions in summary reports, and
        /// not the transaction children results.
        /// </summary>
        /// <returns>the controller for further configuration or usage.</returns>
        public DslTransactionController GenerateParentSample() =>
            GenerateParentSample(true);

        /// <summary>
        /// Same as <see cref="GenerateParentSample()"/> but allowing to enable or disable it.
        /// <br/>
        /// This is helpful when the resolution is taken at runtime.
        /// </summary>
        /// <param name="enable">specifies to enable or disable the setting. By default, it is set to false.</param>
        /// <returns>the controller for further configuration or usage.</returns>
        /// <seealso cref="GenerateParentSample()"/>
        public DslTransactionController GenerateParentSample(bool enable)
        {
            _generateParentSample = enable;
            return this;
        }
    }
}