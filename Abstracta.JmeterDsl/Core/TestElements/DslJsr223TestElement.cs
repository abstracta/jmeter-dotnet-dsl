namespace Abstracta.JmeterDsl.Core.TestElements
{
    /// <summary>
    /// Abstracts common logic used by JSR223 test elements.
    /// </summary>
    public abstract class DslJsr223TestElement<T> : BaseTestElement
    where T : DslJsr223TestElement<T>
    {
        protected readonly string _script;
        protected string _language;

        public DslJsr223TestElement(string name, string script)
            : base(name)
        {
            _script = script;
        }

        public T Language(string language)
        {
            _language = language;
            return (T)this;
        }
    }
}