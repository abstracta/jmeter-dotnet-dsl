namespace Abstracta.JmeterDsl.Core.Timers
{
    /// <summary>
    /// Allows using JMeter Constant Timers which pause the thread for a given period.
    /// <br/>
    /// The pause calculated by the timer will be applied after samplers pre-processors execution and
    /// before actual sampling.
    /// <br/>
    /// Take into consideration that timers applies to all samplers in their scope: if added at test plan
    /// level, it will apply to all samplers in test plan; if added at thread group level, it will apply
    /// only to samples in such thread group; if added as child of a sampler, it will only apply to that
    /// sampler.
    /// </summary>
    public class DslConstantTimer : BaseTimer
    {
        private readonly string _duration;

        public DslConstantTimer(string duration)
            : base(null)
        {
            _duration = duration;
        }
    }
}
