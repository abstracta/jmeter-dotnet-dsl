namespace Abstracta.JmeterDsl.Core.Timers
{
    /// <summary>
    /// Uses JMeter Synchronizing Timer to allow sending a batch of requests simultaneously to a system
    /// under test.
    /// </summary>
    public class DslSynchronizingTimer : BaseTimer
    {
        public DslSynchronizingTimer()
            : base(null)
        {
        }
    }
}
