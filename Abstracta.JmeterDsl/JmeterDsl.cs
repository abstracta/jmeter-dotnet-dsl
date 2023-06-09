using System;
using Abstracta.JmeterDsl.Core;
using Abstracta.JmeterDsl.Core.Listeners;
using Abstracta.JmeterDsl.Core.Samplers;
using Abstracta.JmeterDsl.Core.ThreadGroups;
using Abstracta.JmeterDsl.Http;

namespace Abstracta.JmeterDsl
{
    /// <summary>
    /// This is the main class to be imported from any code using JMeter DSL.
    /// <br/>
    /// This class contains factory methods to create DslTestElement instances that allow
    /// specifying test plans and associated test elements(samplers, thread groups, listeners, etc.).
    /// If you want to support new test elements, then you either add them here (if they are considered
    /// to be part of the core of JMeter), or implement another similar class containing only the
    /// specifics of the protocol, repository, or grouping of test elements that you want to build
    /// (eg, one might implement an Http2JMeterDsl class with only http2 test elements' factory methods).
    /// <br/>
    /// When implementing new factory methods, consider adding only the main properties of the test
    /// elements as parameters (the ones that make sense to specify in most cases). For the rest of the
    /// parameters (the optional ones), prefer specifying them as methods of the implemented
    /// DslTestElement for such cases, in a similar fashion as the Builder Pattern.
    /// </summary>
    public static class JmeterDsl
    {
        /// <summary>
        /// Builds a new test plan.
        /// </summary>
        /// <param name="children">The list of test elements that compose the test plan.</param>
        /// <returns>The test plan instance.</returns>
        /// <seealso cref="DslTestPlan"/>
        public static DslTestPlan TestPlan(params ITestPlanChild[] children) =>
            new DslTestPlan(children);

        /// <summary>
        /// Builds a new thread group with a given number of threads &amp; iterations.
        /// </summary>
        /// <param name="threads">Specifies the number of threads to simulate concurrent virtual users.</param>
        /// <param name="iterations">Specifies the number of iterations that each virtual user will run of children elements until it stops.
        /// If you specify -1, then threads will iterate until test plan execution is interrupted (you manually stop the running process, there is an error and thread group is configured to stop on error, or some other explicit termination condition).
        /// <b>Setting this property to -1 is in general not advised</b>, since you might inadvertently end up running a test plan without limits consuming unnecessary computing power. Prefer specifying a big value as a safe limit for iterations or duration instead.</param>
        /// <param name="children">Contains the test elements that each thread will execute in each iteration.</param>
        /// <returns>The thread group instance.</returns>
        /// <seealso cref="DslThreadGroup"/>
        public static DslThreadGroup ThreadGroup(int threads, int iterations, params IThreadGroupChild[] children) =>
            ThreadGroup(null, threads, iterations, children);

        /// <summary>
        /// Same as <see cref="ThreadGroup(int, int, IThreadGroupChild[])"/> but allowing to set a name on the thread group.
        /// <br/>
        /// Setting a proper name allows to properly identify the requests generated in each thread group.
        /// </summary>
        /// <seealso cref="ThreadGroup(int, int, IThreadGroupChild[])"/>
        public static DslThreadGroup ThreadGroup(string name, int threads, int iterations, params IThreadGroupChild[] children) =>
            new DslThreadGroup(name, threads, iterations, children);

        /// <summary>
        /// Builds a new thread group with a given number of threads &amp; their duration.
        /// </summary>
        /// <param name="threads">to simulate concurrent virtual users.</param>
        /// <param name="duration">to keep each thread running for this period of time. Take into consideration
        /// that JMeter supports specifying duration in seconds, so if you specify a
        /// smaller granularity (like milliseconds) it will be rounded up to seconds.</param>
        /// <param name="children">contains the test elements that each thread will execute until specified
        /// duration is reached.</param>
        /// <returns>the thread group instance.</returns>
        /// <seealso cref="DslThreadGroup"/>
        public static DslThreadGroup ThreadGroup(int threads, TimeSpan duration, params IThreadGroupChild[] children) =>
            ThreadGroup(null, threads, duration, children);

        /// <summary>
        /// Same as <see cref="ThreadGroup(int, TimeSpan, IThreadGroupChild[])"/> but allowing to set a name on the thread group.
        /// <br/>
        /// Setting a proper name allows to properly identify the requests generated in each thread group.
        /// </summary>
        /// <seealso cref="ThreadGroup(int, TimeSpan, IThreadGroupChild[])"/>
        public static DslThreadGroup ThreadGroup(string name, int threads, TimeSpan duration, params IThreadGroupChild[] children) =>
            new DslThreadGroup(name, threads, duration, children);

        /// <summary>
        /// Builds an HTTP Request sampler to sample HTTP requests.
        /// </summary>
        /// <param name="url">Specifies URL the HTTP Request sampler will hit.</param>
        /// <returns>The HTTP Request sampler instance which can be used to define additional settings for
        /// the HTTP request (like method, body, headers, pre &amp; post processors, etc.).</returns>
        /// <seealso cref="DslHttpSampler"/>
        public static DslHttpSampler HttpSampler(string url) =>
            HttpSampler(null, url);

        /// <summary>
        /// Same as <see cref="HttpSampler(string)"/> but allowing to set a name to the HTTP Request sampler.
        /// <br/>
        /// Setting a proper name allows to easily identify the requests generated by this sampler and check its particular statistics.
        /// </summary>
        /// <seealso cref="HttpSampler(string)"/>
        public static DslHttpSampler HttpSampler(string name, string url) =>
            new DslHttpSampler(name, url);

        /// <summary>
        /// Builds an HTTP header manager which allows setting HTTP headers to be used by HTTPRequest
        /// samplers.
        /// </summary>
        /// <returns>the HTTP header manager instance which allows specifying the particular HTTP headers to
        /// use.</returns>
        /// <seealso cref="Http.HttpHeaders"/>
        public static HttpHeaders HttpHeaders() =>
            new HttpHeaders();

        /// <summary>
        /// Builds a Cookie manager at the test plan level which allows configuring cookies settings used
        /// by HTTPRequest samplers.
        /// </summary>
        /// <returns>the http cookies instance which allows configuring cookies settings.</returns>
        /// <seealso cref="DslHttpCookies"/>
        public static DslHttpCookies HttpCookies() =>
            new DslHttpCookies();

        /// <summary>
        /// Builds a Cache manager at the test plan level which allows configuring caching behavior used by
        /// HTTPRequest samplers.
        /// </summary>
        /// <returns>the http cache instance which allows configuring caching settings.</returns>
        /// <seeal cref="DslHttpCache"/>
        public static DslHttpCache HttpCache() =>
            new DslHttpCache();

        /// <summary>
        /// Builds a JMeter plugin Dummy Sampler which allows emulating a sampler easing testing other
        /// parts of a test plan (like extractors, controllers conditions, etc).
        /// <br/>
        /// Usually you would replace an existing sampler with this one, to test some extractor or test
        /// plan complex behavior (like controllers conditions), and once you have verified that the rest
        /// of the plan works as expected, you place back the original sampler that makes actual
        /// interactions to a server.
        /// <br/>
        /// By default, this sampler, in contrast to the JMeter plugin Dummy Sampler, does not simulate
        /// response time. This helps speeding up the debug and tracing process while using it.
        /// </summary>
        /// <param name="responseBody">specifies the response body to be included in generated sample results.</param>
        /// <returns>the dummy sampler for further configuration and usage in test plan.</returns>
        /// <seealso cref="DslDummySampler"/>
        public static DslDummySampler DummySampler(string responseBody)
            => DummySampler(null, responseBody);

        /// <summary>
        /// Same as <see cref="DummySampler(string)"/> but allowing to set a name on the sampler.
        /// <br/>
        /// Setting the name of the sampler allows better simulation the final use case when dummy sampler
        /// is replaced by actual/final sampler, when sample results are reported in stats, logs, etc.
        /// </summary>
        /// <seealso cref="DslDummySampler"/>
        /// <seealso cref="DummySampler(string)"/>
        public static DslDummySampler DummySampler(string name, string responseBody)
            => new DslDummySampler(name, responseBody);

        /// <summary>
        /// Builds a Simple Data Writer to write all collected results to a JTL file.
        /// <br/>
        /// This is just a handy short way of generating JTL files using as filename the template:
        /// <code>&lt;yyyy-MM-dd HH-mm-ss&gt; &lt;UUID&gt;.jtl</code>
        /// <br/>
        /// If you need to have a predictable name, consider using <see cref="JtlWriter(string, string)"/>
        /// instead.
        /// </summary>
        /// <param name="directory">specifies the directory path where jtl files will be generated in. If the
        /// directory does not exist, then it will be created.</param>
        /// <returns>the JtlWriter instance</returns>
        /// <seealso cref="JtlWriter(string, string)"/>
        /// <seealso cref="Core.Listeners.JtlWriter"/>
        public static JtlWriter JtlWriter(string directory) =>
            new JtlWriter(directory, null);

        /// <summary>
        /// Builds a Simple Data Writer to write all collected results to a JTL file.
        /// <br/>
        /// This is particularly helpful when you need to control de file name to do later post-processing
        /// on the file (eg: use CI build ID in the file name).
        /// </summary>
        /// <param name="directory">specifies the directory path where jtl file will be generated. If the
        /// directory does not exist, then it will be created.</param>
        /// <param name="fileName">the name to be used for the file.<b>File names should be unique, otherwise
        /// the new results will be appended to existing file.</b></param>
        /// <returns>the JtlWriter instance</returns>
        public static JtlWriter JtlWriter(string directory, string fileName) =>
            new JtlWriter(directory, fileName);

        /// <summary>
        /// Builds a Response File Saver to generate a file for each response of a sample.
        /// </summary>
        /// <param name="fileNamePrefix">the prefix to be used when generating the files. This should contain the
        /// directory location where the files should be generated and can contain a
        /// file name prefix for all file names (eg: target/response-files/response-).</param>
        /// <returns>the ResponseFileSaver instance.</returns>
        /// <seealso cref="Core.Listeners.ResponseFileSaver"/>
        public static ResponseFileSaver ResponseFileSaver(string fileNamePrefix) =>
            new ResponseFileSaver(fileNamePrefix);

        /// <summary>
        /// Builds a View Results Tree element to show live results in a pop-up window while the test
        /// runs.
        /// <br/>
        /// This element is helpful when debugging a test plan to verify each sample result, and general
        /// structure of results.
        /// </summary>
        /// <returns>the View Results Tree element.</returns>
        /// <seealso cref="Core.Listeners.ResultsTreeVisualizer"/>
        public static ResultsTreeVisualizer ResultsTreeVisualizer() =>
            new ResultsTreeVisualizer();
    }
}
