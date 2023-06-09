using System;

namespace Abstracta.JmeterDsl.Core.Stats
{
    /// <summary>
    /// Provides summary data for a set of timing values.
    /// </summary>
    public class TimeMetricSummary
    {
        /// <summary>
        /// Gets the minimum collected value.
        /// </summary>
        public TimeSpan Min { get; set; }

        /// <summary>
        /// Gets the maximum collected value.
        /// </summary>
        public TimeSpan Max { get; set; }

        /// <summary>
        /// Gets the mean/average of collected values.
        /// </summary>
        public TimeSpan Mean { get; set; }

        /// <summary>
        /// Gets the median of collected values.
        /// <br/>
        /// The median is the same as percentile 50, and is the value for which 50% of the collected values
        /// is smaller/greater.
        /// This value might differ from <see cref="Mean"/> when distribution of values is not symmetric.
        /// </summary>
        public TimeSpan Median { get; set; }

        /// <summary>
        /// Gets the 90 percentile of samples times.
        /// <br/>
        /// 90% of samples took less or equal to the returned value.
        /// </summary>
        public TimeSpan Perc90 { get; set; }

        /// <summary>
        /// Gets the 95 percentile of samples times.
        /// <br/>
        /// 95% of samples took less or equal to the returned value.
        /// </summary>
        public TimeSpan Perc95 { get; set; }

        /// <summary>
        /// Gets the 99 percentile of samples times.
        /// <br/>
        /// 99% of samples took less or equal to the returned value.
        /// </summary>
        public TimeSpan Perc99 { get; set; }
    }
}
