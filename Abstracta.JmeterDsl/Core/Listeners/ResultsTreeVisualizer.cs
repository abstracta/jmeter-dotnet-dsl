namespace Abstracta.JmeterDsl.Core.Listeners
{
    /// <summary>
    /// Shows a popup window including live results tree using JMeter built-in View Results Tree
    /// element.
    /// <br/>
    /// If resultsTreeVisualizer is added at testPlan level it will show information about all samples in
    /// the test plan, if added at thread group level it will only show samples for samplers contained
    /// within it, if added as a sampler child, then only that sampler samples will be shown.
    /// </summary>
    public class ResultsTreeVisualizer : BaseListener
    {
        protected int? _resultsLimit;

        /// <summary>
        /// Specifies the maximum number of sample results to show.
        /// <br/>
        /// When the limit is reached, only latest sample results are shown.
        /// <br/>
        /// Take into consideration that the greater the number of displayed results, the more system
        /// memory is required, which might cause an OutOfMemoryError depending on JVM settings.
        /// </summary>
        /// <param name="resultsLimit">the maximum number of sample results to show. When not set the default
        /// value is 500.</param>
        /// <returns>the visualizer for further configuration or usage.</returns>
        public ResultsTreeVisualizer ResultsLimit(int resultsLimit)
        {
            _resultsLimit = resultsLimit;
            return this;
        }
    }
}
