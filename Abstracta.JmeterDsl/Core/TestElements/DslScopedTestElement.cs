namespace Abstracta.JmeterDsl.Core.TestElements
{
    /// <summary>
    /// Contains common logic for test elements that only process certain samples.
    /// </summary>
    /// <typeparam name="T">is the type of the test element that extends this class (to properly inherit fluent
    /// API methods).</typeparam>
    public abstract class DslScopedTestElement<T> : BaseTestElement, IMultiLevelTestElement
        where T : DslScopedTestElement<T>
    {
        protected DslScope? _scope;
        protected string _scopeVariable;

        protected DslScopedTestElement(string name)
            : base(name)
        {
        }

        public enum DslScope
        {
            /// <summary>
            /// Applies the regular extractor to all samples (main and sub samples).
            /// </summary>
            AllSamples,

            /// <summary>
            /// Applies the regular extractor only to main sample (sub samples, like redirects, are not
            /// included).
            /// </summary>
            MainSample,

            /// <summary>
            /// Applies the regular extractor only to sub samples (redirects, embedded resources, etc.).
            /// </summary>
            SubSamples,
        }

        /// <summary>
        /// Allows specifying if the element should be applied to main sample and/or sub samples.
        /// <br/>
        /// When not specified the element will only apply to main sample.
        /// </summary>
        /// <param name="scope">specifying to what sample result apply the element to.</param>
        /// <returns>the DSL element for further configuration or usage.</returns>
        /// <seealso cref="Scope"/>
        public T Scope(DslScope scope)
        {
            _scope = scope;
            return (T)this;
        }

        /// <summary>
        /// Allows specifying that the element should be applied to the contents of a given JMeter
        /// variable.
        /// <br/>
        /// This setting overrides any setting on scope and fieldToCheck.
        /// </summary>
        /// <param name="scopeVariable">specifies the name of the variable to apply the element to.</param>
        /// <returns>the DSL element for further configuration or usage.</returns>
        public T ScopeVariable(string scopeVariable)
        {
            _scopeVariable = scopeVariable;
            return (T)this;
        }
    }
}