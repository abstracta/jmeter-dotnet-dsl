using System;

namespace Abstracta.JmeterDsl.Core.Stats
{
    /// <summary>
    /// Contains summary statistics of a group of collected sample results.
    /// </summary>
    public class StatsSummary
    {
        /// <summary>
        /// Gets the instant when the first sample started.
        /// <br/>
        /// When associated to a test plan or transaction it gets its start time.
        /// </summary>
        public DateTime FirstTime { get; set; }

        /// <summary>
        /// Gets the instant when the last sample ended.
        /// <br/>
        /// When associated to a test plan or transaction it gets its end time.
        /// <br/>
        /// Take into consideration that for transactions this time takes not only into consideration the
        /// endTime of last sample, but also the time spent in timers and pre and postprocessors.
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// Gets metrics for number of samples
        /// <br/>
        /// This counts both failing and passing samples.
        /// </summary>
        public CountMetricSummary Samples { get; set; }

        /// <summary>
        /// Gets the total number of samples.
        /// </summary>
        public long SamplesCount => Samples.Total;

        /// <summary>
        /// Gets metrics for number of samples that failed.
        /// </summary>
        public CountMetricSummary Errors { get; set; }

        /// <summary>
        /// Gets the total number of samples that failed.
        /// </summary>
        public long ErrorsCount => Errors.Total;

        /// <summary>
        /// Gets metrics for time spent in samples.
        /// </summary>
        public TimeMetricSummary SampleTime { get; set; }

        /// <summary>
        /// Gets the 99 percentile of samples times.
        /// <br/>
        /// 99% of samples took less or equal to the returned value.
        /// </summary>
        public TimeSpan SampleTimePercentile99 => SampleTime.Perc99;

        /// <summary>
        /// Gets metrics for received bytes in sample responses.
        /// </summary>
        public CountMetricSummary ReceivedBytes { get; set; }

        /// <summary>
        /// Gets metrics for sent bytes in samples requests.
        /// </summary>
        public CountMetricSummary SentBytes { get; set; }
    }
}
