namespace Abstracta.JmeterDsl.Core.Stats
{
    /// <summary>
    /// Provides summary data for a set of count values.
    /// </summary>
    public class CountMetricSummary
    {
        /// <summary>
        /// Provides the total count (the sum).
        /// </summary>
        public long Total { get; set; }

        /// <summary>
        /// Provides the average count per second for the given metric.
        /// </summary>
        public double PerSecond { get; set; }
    }
}
