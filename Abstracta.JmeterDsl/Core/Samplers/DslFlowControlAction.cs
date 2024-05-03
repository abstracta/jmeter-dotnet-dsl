using Abstracta.JmeterDsl.Core.Bridge;

namespace Abstracta.JmeterDsl.Core.Samplers
{
    /// <summary>
    /// Uses JMeter Flow Control Action to allow taking different actions (stop, pause, interrupt).
    /// </summary>
    [YamlType(TagName = "threadPause")]
    public class DslFlowControlAction : BaseSampler<DslFlowControlAction>
    {
        private readonly string _duration;

        private DslFlowControlAction(string duration)
            : base(null)
        {
            _duration = duration;
        }

        public static DslFlowControlAction PauseThread(string duration) =>
            new DslFlowControlAction(duration);
    }
}