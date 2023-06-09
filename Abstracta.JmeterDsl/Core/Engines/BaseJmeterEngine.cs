using Abstracta.JmeterDsl.Core.Bridge;
using YamlDotNet.Serialization;

namespace Abstracta.JmeterDsl.Core.Engines
{
    public abstract class BaseJmeterEngine<T> : IDslJmeterEngine
        where T : BaseJmeterEngine<T>
    {
        [YamlIgnore]
        protected string _jvmArgs = string.Empty;

        /// <summary>
        /// Specifies arguments to be added to JVM command line.
        /// <br/>
        /// This is helpful, for example, when debugging JMeter DSL or JMeter code.
        /// </summary>
        public T JvmArgs(string args)
        {
            _jvmArgs = args;
            return (T)this;
        }

        public TestPlanStats Run(DslTestPlan testPlan) =>
            new BridgeService()
                .JvmArgs(_jvmArgs)
                .RunTestPlanInEngine(testPlan, this);
    }
}
