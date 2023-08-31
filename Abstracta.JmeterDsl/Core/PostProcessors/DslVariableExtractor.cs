using Abstracta.JmeterDsl.Core.TestElements;

namespace Abstracta.JmeterDsl.Core.PostProcessors
{
    /// <summary>
    /// Contains common logic for post processors which extract some value into a variable.
    /// </summary>
    public abstract class DslVariableExtractor<T> : DslScopedTestElement<T>
        where T : DslVariableExtractor<T>
    {
        protected readonly string _variableName;
        protected int? _matchNumber;
        protected string _defaultValue;

        public DslVariableExtractor(string name, string varName)
            : base(name)
        {
            _variableName = varName;
        }
    }
}