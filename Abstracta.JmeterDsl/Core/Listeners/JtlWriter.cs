namespace Abstracta.JmeterDsl.Core.Listeners
{
    /// <summary>
    /// Allows to generate a result log file (JTL) with data for each sample for a test plan, thread
    /// group or sampler, depending on what level of test plan is added.
    /// <br/>
    /// If jtlWriter is added at testPlan level it will log information about all samples in the test
    /// plan, if added at thread group level it will only log samples for samplers contained within it,
    /// if added as a sampler child, then only that sampler samples will be logged.
    /// <br/>
    /// By default, this writer will use JMeter default JTL format, a csv with following fields:
    /// timeStamp,elapsed,label,responseCode,responseMessage,threadName,dataType,success,failureMessage,
    /// bytes,sentBytes,grpThreads,allThreads,URL,Latency,IdleTime,Connect. You can change the format to
    /// XML and specify additional (or remove existing ones) fields to store with provided methods.
    /// <br/>
    /// See <a href="http://jmeter.apache.org/usermanual/listeners.html">JMeter listeners doc</a> for
    /// more details on JTL format and settings.
    /// </summary>
    ///
    public class JtlWriter : BaseListener
    {
        private readonly string _directory;
        private readonly string _fileName;
        private bool? _withAllFields;
        private bool? _saveAsXml;
        private bool? _withElapsedTime;
        private bool? _withResponseMessage;
        private bool? _withSuccess;
        private bool? _withSentByteCount;
        private bool? _withResponseFilename;
        private bool? _withEncoding;
        private bool? _withIdleTime;
        private bool? _withResponseHeaders;
        private bool? _withAssertionResults;
        private bool? _withFieldNames;
        private bool? _withLabel;
        private bool? _withThreadName;
        private bool? _withAssertionFailureMessage;
        private bool? _withActiveThreadCounts;
        private bool? _withLatency;
        private bool? _withSampleAndErrorCounts;
        private bool? _withRequestHeaders;
        private bool? _withResponseData;
        private bool? _withTimeStamp;
        private bool? _withResponseCode;
        private bool? _withDataType;
        private bool? _withReceivedByteCount;
        private bool? _withUrl;
        private bool? _withConnectTime;
        private bool? _withHostname;
        private bool? _withSamplerData;
        private bool? _withSubResults;
        private string[] _withVariables;

        public JtlWriter(string directory, string fileName)
        {
            _directory = directory;
            _fileName = fileName;
        }

        /// <summary>
        /// Allows setting to include all fields in XML format.
        /// <br/>
        /// This is just a shorter way of using <see cref="WithAllFields(bool)"/> with true setting.
        /// </summary>
        /// <returns>the JtlWriter for further configuration or usage.</returns>
        /// <seealso cref="WithAllFields(bool)"/>
        public JtlWriter WithAllFields() =>
            WithAllFields(true);

        /// <summary>
        /// Allows setting if all or none fields are enabled when saving the JTL.
        /// <br/>
        /// If you enable them all, then XML format will be used.
        /// <br/>
        /// Take into consideration that having a JTL writer with no fields enabled makes no sense. But,
        /// you may want to disable all fields to then enable specific ones, and not having to manually
        /// disable each of default included fields manually. The same applies when you want most of the
        /// fields except for some: in such case you can enable all and then manually disable the ones that
        /// you want to exclude.
        /// <br/>
        /// Also take into consideration that the more fields you add to JTL writer, the more time JMeter
        /// will spend on saving the information, and the more disk the file will consume. So, include
        /// fields thoughtfully.
        /// </summary>
        /// <param name="enabled">specifies whether enable or disable all fields.</param>
        /// <returns>the JtlWriter for further configuration or usage.</returns>
        public JtlWriter WithAllFields(bool enabled)
        {
            _withAllFields = enabled;
            return this;
        }

        /// <summary>
        /// Allows specifying to use XML or CSV format for saving JTL.
        /// <br/>
        /// Take into consideration that some fields (like requestHeaders, responseHeaders, etc.) will only
        /// be saved when XML format is used.
        /// </summary>
        /// <param name="enabled">specifies whether enable XML format saving, or disable it (and use CSV). By
        /// default, it is set to false.</param>
        /// <returns>the JtlWriter for further configuration or usage.</returns>
        public JtlWriter SaveAsXml(bool enabled)
        {
            _saveAsXml = enabled;
            return this;
        }

        /// <summary>
        /// Allows setting whether or not to include elapsed time (milliseconds spent in each sample) in
        /// generated JTL.
        /// <br/>
        /// This is usually the most important metric to collect during a performance test, so in general
        /// this should be included.
        /// </summary>
        /// <param name="enabled">specifies whether enable or disable inclusion of elapsed time. By default, it is
        /// set to true.</param>
        /// <returns>the JtlWriter for further configuration or usage.</returns>
        public JtlWriter WithElapsedTime(bool enabled)
        {
            _withElapsedTime = enabled;
            return this;
        }

        /// <summary>
        /// Allows setting whether or not to include response message (eg: "OK" for HTTP 200 status code)
        /// in generated JTL.
        /// <br/>
        /// This property is usually handy to trace potential issues, specially the ones that are not
        /// standard issues (like HTTPConnectionExceptions) which are not deducible from response code.
        /// </summary>
        /// <param name="enabled">specifies whether enable or disable inclusion of response message. By default,
        /// it is set to true.</param>
        /// <returns>the JtlWriter for further configuration or usage.</returns>
        public JtlWriter WithResponseMessage(bool enabled)
        {
            _withResponseMessage = enabled;
            return this;
        }

        /// <summary>
        /// Allows setting whether or not to include success (a bool indicating if request was success
        /// or not) field in generated JTL.
        /// <br/>
        /// This property is usually handy to easily identify if a request failed or not (either due to
        /// default JMeter logic, or due to some assertion check or post processor alteration).
        /// </summary>
        /// <param name="enabled">specifies whether enable or disable inclusion of success field. By default, it
        /// is set to true.</param>
        /// <returns>the JtlWriter for further configuration or usage.</returns>
        public JtlWriter WithSuccess(bool enabled)
        {
            _withSuccess = enabled;
            return this;
        }

        /// <summary>
        /// Allows setting whether or not to include sent bytes count (number of bytes sent to server by
        /// request) field in generated JTL.
        /// <br/>
        /// This property is helpful when requests are dynamically generated or when you want to easily
        /// evaluate how much data/load has been transferred to the server.
        /// </summary>
        /// <param name="enabled">specifies whether enable or disable inclusion of sent bytes count. By default,
        /// it is set to true.</param>
        /// <returns>the JtlWriter for further configuration or usage.</returns>
        public JtlWriter WithSentByteCount(bool enabled)
        {
            _withSentByteCount = enabled;
            return this;
        }

        /// <summary>
        /// Allows setting whether or not to include response file name (name of file stored by
        /// <see cref="ResponseFileSaver"/>) field in generated JTL.
        /// <br/>
        /// This property is helpful when ResponseFileSaver is used to easily trace the request response
        /// contents and don't have to include them in JTL file itself.
        /// </summary>
        /// <param name="enabled">specifies whether enable or disable inclusion of response file name. By default,
        /// it is set to false.</param>
        /// <returns>the JtlWriter for further configuration or usage.</returns>
        public JtlWriter WithResponseFilename(bool enabled)
        {
            _withResponseFilename = enabled;
            return this;
        }

        /// <summary>
        /// Allows setting whether or not to include the response encoding (eg: UTF-8, ISO-8859-1, etc.)
        /// field in generated JTL.
        /// </summary>
        /// <param name="enabled">specifies whether enable or disable inclusion of response encoding. By default,
        /// it is set to false.</param>
        /// <returns>the JtlWriter for further configuration or usage.</returns>
        public JtlWriter WithEncoding(bool enabled)
        {
            _withEncoding = enabled;
            return this;
        }

        /// <summary>
        /// Allows setting whether or not to include the Idle time (milliseconds spent in JMeter
        /// processing, but not sampling, generally 0) field in generated JTL.
        /// </summary>
        /// <param name="enabled">specifies whether enable or disable inclusion of idle time. By default, it is
        /// set to true.</param>
        /// <returns>the JtlWriter for further configuration or usage.</returns>
        public JtlWriter WithIdleTime(bool enabled)
        {
            _withIdleTime = enabled;
            return this;
        }

        /// <summary>
        /// Allows setting whether or not to include response headers (eg: HTTP headers like Content-Type
        /// and the like) field in generated JTL.
        /// <br/>
        /// <b>Note:</b> this field will only be saved if <see cref="SaveAsXml(bool)"/> is also set to
        /// true.
        /// </summary>
        /// <param name="enabled">specifies whether enable or disable inclusion of response headers. By default,
        /// it is set to false.</param>
        /// <returns>the JtlWriter for further configuration or usage.</returns>
        public JtlWriter WithResponseHeaders(bool enabled)
        {
            _withResponseHeaders = enabled;
            return this;
        }

        /// <summary>
        /// Allows setting whether or not to include assertion results (with name, success field, and
        /// potential error message) info in generated JTL.
        /// <br/>
        /// <b>Note:</b> this will only be saved if <see cref="SaveAsXml(bool)"/> is also set to
        /// true.
        /// <br/>
        /// This info is handy when tracing why requests are marked as failure and exact reason.
        /// </summary>
        /// <param name="enabled">specifies whether enable or disable inclusion of assertion results. By default,
        /// it is set to true.</param>
        /// <returns>the JtlWriter for further configuration or usage.</returns>
        public JtlWriter WithAssertionResults(bool enabled)
        {
            _withAssertionResults = enabled;
            return this;
        }

        /// <summary>
        /// Allows setting whether or not to include assertion results (with name, success field, and
        /// potential error message) info in generated JTL.
        /// <br/>
        /// <b>Note:</b> this will only be saved if <see cref="SaveAsXml(bool)"/> is set to false (or not
        /// set, which defaults XML save to false).
        /// </summary>
        /// <param name="enabled">specifies whether enable or disable inclusion of assertion results. By default,
        /// it is set to true.</param>
        /// <returns>the JtlWriter for further configuration or usage.</returns>
        public JtlWriter WithFieldNames(bool enabled)
        {
            _withFieldNames = enabled;
            return this;
        }

        /// <summary>
        /// Allows setting whether or not to include sample label (i.e.: name of the request) field in
        /// generated JTL.
        /// <br/>
        /// In general, you should enable this field to properly identify results to associated samplers.
        /// </summary>
        /// <param name="enabled">specifies whether enable or disable inclusion of sample labels. By default, it
        /// is set to true.</param>
        /// <returns>the JtlWriter for further configuration or usage.</returns>
        public JtlWriter WithLabel(bool enabled)
        {
            _withLabel = enabled;
            return this;
        }

        /// <summary>
        /// Allows setting whether or not to include thread name field in generated JTL.
        /// <br/>
        /// This is helpful to identify the requests generated by each thread and allow tracing
        /// "correlated" requests (requests that are associated to previous requests in same thread).
        /// </summary>
        /// <param name="enabled">specifies whether enable or disable inclusion of thread name. By default, it is
        /// set to true.</param>
        /// <returns>the JtlWriter for further configuration or usage.</returns>
        public JtlWriter WithThreadName(bool enabled)
        {
            _withThreadName = enabled;
            return this;
        }

        /// <summary>
        /// Allows setting whether or not to include assertion failure message field in generated JTL.
        /// <br/>
        /// This is helpful to trace potential reason of a request being marked as failure.
        /// </summary>
        /// <param name="enabled">specifies whether enable or disable inclusion of assertion failure message. By
        /// default, it is set to true.</param>
        /// <returns>the JtlWriter for further configuration or usage.</returns>
        public JtlWriter WithAssertionFailureMessage(bool enabled)
        {
            _withAssertionFailureMessage = enabled;
            return this;
        }

        /// <summary>
        /// Allows setting whether or not to include active thread counts (basically, number of concurrent
        /// requests, both in the sample thread group, and in all thread groups) fields in generated JTL.
        /// <br/>
        /// This is helpful to know under how much load (concurrent requests) is the tested service at the
        /// moment the request was done.
        /// </summary>
        /// <param name="enabled">specifies whether enable or disable inclusion of active thread counts. By
        /// default, it is set to true.</param>
        /// <returns>the JtlWriter for further configuration or usage.</returns>
        public JtlWriter WithActiveThreadCounts(bool enabled)
        {
            _withActiveThreadCounts = enabled;
            return this;
        }

        /// <summary>
        /// Allows setting whether or not to include latency time (milliseconds between the sample started
        /// and first byte of response is received) field in generated JTL.
        /// <br/>
        /// This is usually helpful to identify how fast does the tested service takes to answer, taking
        /// out the time spent in transferring response data.
        /// </summary>
        /// <param name="enabled">specifies whether enable or disable inclusion of latency time. By default, it is
        /// set to true.</param>
        /// <returns>the JtlWriter for further configuration or usage.</returns>
        public JtlWriter WithLatency(bool enabled)
        {
            _withLatency = enabled;
            return this;
        }

        /// <summary>
        /// Allows setting whether or not to include sample counts (total and error counts) fields in
        /// generated JTL.
        /// <br/>
        /// In general sample count will be 1, and error count will be 0 or 1 depending on sample success
        /// or failure. But there are some scenarios where these counts might be greater, for example when
        /// controllers results are being included.
        /// </summary>
        /// <param name="enabled">specifies whether enable or disable inclusion of sample counts. By default, it
        /// is set to false.</param>
        /// <returns>the JtlWriter for further configuration or usage.</returns>
        public JtlWriter WithSampleAndErrorCounts(bool enabled)
        {
            _withSampleAndErrorCounts = enabled;
            return this;
        }

        /// <summary>
        /// Allows setting whether or not to include request headers (eg: HTTP headers like User-Agent and
        /// the like) field in generated JTL.
        /// <br/>
        /// <b>Note:</b> this field will only be saved if <see cref="SaveAsXml(bool)"/> is also set to
        /// true.
        /// </summary>
        /// <param name="enabled">specifies whether enable or disable inclusion of request headers. By default, it
        /// is set to false.</param>
        /// <returns>the JtlWriter for further configuration or usage.</returns>
        public JtlWriter WithRequestHeaders(bool enabled)
        {
            _withRequestHeaders = enabled;
            return this;
        }

        /// <summary>
        /// Allows setting whether or not to include response body field in generated JTL.
        /// <br/>
        /// <b>Note:</b> this field will only be saved if <see cref="SaveAsXml(bool)"/> is also set to
        /// true.
        /// <br/>
        /// This is usually helpful for tracing the response obtained by each sample. Consider using
        /// <see cref="ResponseFileSaver"/> to get a file for each response body.
        /// </summary>
        /// <param name="enabled">specifies whether enable or disable inclusion of response body. By default, it
        /// is set to false.</param>
        /// <returns>the JtlWriter for further configuration or usage.</returns>
        public JtlWriter WithResponseData(bool enabled)
        {
            _withResponseData = enabled;
            return this;
        }

        /// <summary>
        /// Allows setting whether or not to include timestamp (epoch when the sample started) field in
        /// generated JTL.
        /// </summary>
        /// <param name="enabled">specifies whether enable or disable inclusion of timestamps. By default, it is
        /// set to true.</param>
        /// <returns>the JtlWriter for further configuration or usage.</returns>
        public JtlWriter WithTimeStamp(bool enabled)
        {
            _withTimeStamp = enabled;
            return this;
        }

        /// <summary>
        /// Allows setting whether or not to include response codes (e.g.: 200) field in generated JTL.
        /// <br/>
        /// This field allows to quickly identify different reasons for failure in server (eg: bad request,
        /// service temporally unavailable, etc.).
        /// </summary>
        /// <param name="enabled">specifies whether enable or disable inclusion of response codes. By default, it
        /// is set to true.</param>
        /// <returns>the JtlWriter for further configuration or usage.</returns>
        public JtlWriter WithResponseCode(bool enabled)
        {
            _withResponseCode = enabled;
            return this;
        }

        /// <summary>
        /// Allows setting whether or not to include response data type (i.e.: binary or text) field in
        /// generated JTL.
        /// </summary>
        /// <param name="enabled">specifies whether enable or disable inclusion of response data types. By
        /// default, it is set to true.</param>
        /// <returns>the JtlWriter for further configuration or usage.</returns>
        public JtlWriter WithDataType(bool enabled)
        {
            _withDataType = enabled;
            return this;
        }

        /// <summary>
        /// Allows setting whether or not to include received bytes count (number of bytes sent by server
        /// in the response) field in generated JTL.
        /// <br/>
        /// This property is helpful to measure how much load is the network getting and how much
        /// information is the tested service generating.
        /// </summary>
        /// <param name="enabled">specifies whether enable or disable inclusion of received bytes counts. By
        /// default, it is set to true.</param>
        /// <returns>the JtlWriter for further configuration or usage.</returns>
        public JtlWriter WithReceivedByteCount(bool enabled)
        {
            _withReceivedByteCount = enabled;
            return this;
        }

        /// <summary>
        /// Allows setting whether or not to include url field in generated JTL.
        /// <br/>
        /// This property is helpful when URLs are dynamically generated and may vary for the sample
        /// sampler
        /// </summary>
        /// <param name="enabled">specifies whether enable or disable inclusion of urls. By default, it is set to
        /// true.</param>
        /// <returns>the JtlWriter for further configuration or usage.</returns>
        public JtlWriter WithUrl(bool enabled)
        {
            _withUrl = enabled;
            return this;
        }

        /// <summary>
        /// Allows setting whether or not to include connect time (milliseconds between the sample started
        /// and connection is established to service to start sending request) field in generated JTL.
        /// <br/>
        /// This is usually helpful to identify issues in network latency when connecting or server load
        /// when serving connection requests.
        /// </summary>
        /// <param name="enabled">specifies whether enable or disable inclusion of connect time. By default, it is
        /// set to true.</param>
        /// <returns>the JtlWriter for further configuration or usage.</returns>
        public JtlWriter WithConnectTime(bool enabled)
        {
            _withConnectTime = enabled;
            return this;
        }

        /// <summary>
        /// Allows setting whether or not to include host name (name of host that did the sample) field in
        /// generated JTL.
        /// <br/>
        /// This particularly helpful when running JMeter in a distributed fashion to identify which node
        /// the sample result is associated to.
        /// </summary>
        /// <param name="enabled">specifies whether enable or disable inclusion of host names. By default, it is
        /// set to false.</param>
        /// <returns>the JtlWriter for further configuration or usage.</returns>
        public JtlWriter WithHostname(bool enabled)
        {
            _withHostname = enabled;
            return this;
        }

        /// <summary>
        /// Allows setting whether or not to include sampler data (like cookies, HTTP method, request body
        /// and redirection URL) entries in generated JTL.
        /// <br/>
        /// <b>Note:</b> this field will only be saved if <see cref="SaveAsXml(bool)"/> is also set to
        /// true.
        /// </summary>
        /// <param name="enabled">specifies whether enable or disable inclusion of sample data. By default, it is
        /// set to false.</param>
        /// <returns>the JtlWriter for further configuration or usage.</returns>
        public JtlWriter WithSamplerData(bool enabled)
        {
            _withSamplerData = enabled;
            return this;
        }

        /// <summary>
        /// Allows setting whether or not to include sub results (like redirects) entries in generated
        /// JTL.
        /// </summary>
        /// <param name="enabled">specifies whether enable or disable inclusion of sub results. By default, it is
        /// set to true.</param>
        /// <returns>the JtlWriter for further configuration or usage.</returns>
        public JtlWriter WithSubResults(bool enabled)
        {
            _withSubResults = enabled;
            return this;
        }

        /// <summary>
        /// Allows specifying JMeter variables to include in generated jtl file.
        /// <br/>
        /// <b>Warning:</b> variables to sample are test plan wide. This means that if you set them in one
        /// jtl writer, they will appear in all jtl writers used in the test plan. Moreover, if you set
        /// them in different jtl writers, only variables set on latest one will be considered.
        /// </summary>
        /// <param name="variables">names of JMeter variables to include in jtl file.</param>
        /// <returns>the JtlWriter for further configuration or usage.</returns>
        public JtlWriter WithVariables(params string[] variables)
        {
            _withVariables = variables;
            return this;
        }
    }
}
