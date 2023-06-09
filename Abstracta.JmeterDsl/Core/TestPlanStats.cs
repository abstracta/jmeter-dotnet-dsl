using System;
using System.Collections.Generic;
using Abstracta.JmeterDsl.Core.Stats;

namespace Abstracta.JmeterDsl.Core
{
    /// <summary>
    /// Contains all statistics collected during the execution of a test plan.
    /// <br/>
    /// When using different samples, specify different names on them to be able to get each sampler
    /// specific statistics after they run.
    /// </summary>
    public class TestPlanStats
    {
        /// <summary>
        /// Provides the time taken to run the test plan.
        /// </summary>
        public TimeSpan Duration { get; set; }

        /// <summary>
        /// Provides statistics for the entire test plan.
        /// </summary>
        public StatsSummary Overall { get; set; }

        /// <summary>
        /// Provides statistics for each label (usually, samplers labels).
        /// </summary>
        public Dictionary<string, StatsSummary> Labels { get; set; }
    }
}
