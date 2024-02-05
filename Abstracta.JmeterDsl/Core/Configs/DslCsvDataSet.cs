using System.Text;

namespace Abstracta.JmeterDsl.Core.Configs
{
    /// <summary>
    /// Allows using a CSV file as input data for JMeter variables to use in test plan.
    /// <br/>
    /// This element reads a CSV file and uses each line to generate JMeter variables to be used in each
    /// iteration and thread of the test plan.
    /// <br/>
    /// Is ideal to be able to easily create test plans that test with a lot of different of potential
    /// requests or flows.
    /// <br/>
    /// By default, it consumes comma separated variables, which names are included in first line of CSV,
    /// automatically resets to the beginning of the file when the end is reached and the consumption of
    /// the file is shared by all threads and thread groups in the test plan (ie: any iteration on a
    /// thread will consume a line from the file, and advance to following line).
    /// <br/>
    /// Additionally, this element sets by default the "quoted data" flag on JMeter CSV Data Set
    /// element.
    /// </summary>
    public class DslCsvDataSet : BaseConfigElement
    {
        private readonly string _csvFile;
        private string _delimiter;
        private string _encoding;
        private string[] _variableNames;
        private bool? _ignoreFirstLine;
        private bool? _stopThreadOnEOF;
        private Sharing? _sharedIn;
        private bool? _randomOrder;

        public DslCsvDataSet(string csvFile)
            : base(null)
        {
            _csvFile = csvFile;
        }

        /// <summary>
        /// Specifies the way the threads in a test plan consume the CSV.
        /// </summary>
        public enum Sharing
        {
            /// <summary>
            /// All threads in the test plan will share the CSV file, meaning that any thread iteration will
            /// consume an entry from it. You can think as having only one pointer to the current line of the
            /// CSV, being advanced by any thread iteration. The file is only opened once.
            /// </summary>
            AllThreads,

            /// <summary>
            /// CSV file consumption is only shared within thread groups. This means that threads in separate
            /// thread groups will use separate indexes to consume the data. The file is open once per thread
            /// group.
            /// </summary>
            ThreadGroup,

            /// <summary>
            /// CSV file consumption is isolated per thread. This means that each thread will start consuming
            /// the CSV from the beginning and not share any information with other threads. The file is open
            /// once per thread.
            /// </summary>
            Thread,
        }

        /// <summary>
        /// Specifies the delimiter used by the file to separate variable values.
        /// </summary>
        /// <param name="delimiter">specifies the delimiter. By default, it uses commas (,) as delimiters. If you need to use tabs, then specify "\\t".</param>
        /// <returns>the dataset for further configuration or usage.</returns>
        public DslCsvDataSet Delimiter(string delimiter)
        {
            _delimiter = delimiter;
            return this;
        }

        /// <summary>
        /// Specifies the file encoding used by the file.
        /// <br/>
        /// This method is useful when specifying a dynamic encoding (through JMeter variable or function
        /// reference). Otherwise prefer using <see cref="Encoding(Encoding)"/>.
        /// </summary>
        /// <param name="encoding">the file encoding of the file. By default, it will use UTF-8 (which differs
        ///                  from JMeter default, to have more consistent test plan execution). This might
        ///                  require to be changed but in general is good to have all files in same encoding
        ///                  (eg: UTF-8).</param>
        /// <returns>the dataset for further configuration or usage.</returns>
        public DslCsvDataSet Encoding(string encoding)
        {
            _encoding = encoding;
            return this;
        }

        /// <summary>
        /// Specifies the file encoding used by the file.
        /// <br/>
        /// If you need to specify a dynamic encoding (through JMeter variable or function reference), then
        /// use <see cref="Encoding(string)"/> instead.
        /// </summary>
        /// <param name="encoding">the file encoding of the file. By default, it will use UTF-8 (which differs
        ///                  from JMeter default, to have more consistent test plan execution). This might
        ///                  require to be changed but in general is good to have all files in same encoding
        ///                  (eg: UTF-8).</param>
        /// <returns>the dataset for further configuration or usage.</returns>
        public DslCsvDataSet Encoding(Encoding encoding)
        {
            _encoding = encoding.EncodingName;
            return this;
        }

        /// <summary>
        /// Specifies variable names to be assigned to the parsed values.
        /// <br/>
        /// If you have a CSV file with existing headers and want to overwrite the name of generated
        /// variables, then use <see cref="IgnoreFirstLine()"/> in conjunction with this method to specify the
        /// new variable names. If you have a CSV file without a headers line, then you will need to use
        /// this method to set proper names for the variables (otherwise first line of data will be used as
        /// headers, which will not be good).
        /// </summary>
        /// <param name="variableNames">names of variables to be extracted from the CSV file.</param>
        /// <returns>the dataset for further configuration or usage.</returns>
        public DslCsvDataSet VariableNames(params string[] variableNames)
        {
            _variableNames = variableNames;
            return this;
        }

        /// <summary>
        /// Specifies to ignore first line of the CSV.
        /// <br/>
        /// This should only be used in conjunction with <see cref="VariableNames(string[])"/> to overwrite
        /// existing CSV headers names.
        /// </summary>
        /// <returns>the dataset for further configuration or usage.</returns>
        public DslCsvDataSet IgnoreFirstLine()
            => IgnoreFirstLine(true);

        /// <summary>
        /// Same as <see cref="IgnoreFirstLine()"/> but allowing to enable or disable it.
        /// <br/>
        /// This is helpful when the resolution is taken at runtime.
        /// </summary>
        /// <param name="enable">specifies to enable or disable the setting. By default, it is set to false.</param>
        /// <returns>the dataset for further configuration or usage.</returns>
        /// <seealso cref="IgnoreFirstLine()"/>
        public DslCsvDataSet IgnoreFirstLine(bool enable)
        {
            _ignoreFirstLine = enable;
            return this;
        }

        /// <summary>
        /// Specifies to stop threads when end of given CSV file is reached.
        /// <br/>
        /// This method will automatically internally set JMeter test element property "recycle on EOF", so
        /// you don't need to worry about such property.
        /// </summary>
        /// <returns>the dataset for further configuration or usage.</returns>
        public DslCsvDataSet StopThreadOnEOF()
            => StopThreadOnEOF(true);

        /// <summary>
        /// Same as <see cref="StopThreadOnEOF()"/> but allowing to enable or disable it.
        /// <br/>
        /// This is helpful when the resolution is taken at runtime.
        /// </summary>
        /// <param name="enable">specifies to enable or disable the setting. By default, it is set to false.</param>
        /// <returns>the dataset for further configuration or usage.</returns>
        /// <seealso cref="StopThreadOnEOF()"/>
        public DslCsvDataSet StopThreadOnEOF(bool enable)
        {
            _stopThreadOnEOF = enable;
            return this;
        }

        /// <summary>
        /// Allows changing the way CSV file is consumed (shared) by threads.
        /// </summary>
        /// <param name="shareMode">specifies the way threads consume information from the CSV file. By default,
        ///                  all threads share the CSV information, meaning that any thread iteration will
        ///                  advance the consumption of the file (the file is a singleton). When
        ///                  <see cref="RandomOrder()"/> is used, THREAD_GROUP shared mode is not supported.</param>
        /// <returns>the dataset for further configuration or usage.</returns>
        /// <seealso cref="Sharing"/>
        public DslCsvDataSet SharedIn(Sharing shareMode)
        {
            _sharedIn = shareMode;
            return this;
        }

        /// <summary>
        /// Specifies to get file lines in random order instead of sequentially iterating over them.
        /// <br/>
        /// When this method is invoked <a href="https://github.com/Blazemeter/jmeter-bzm-plugins/blob/master/random-csv-data-set/RandomCSVDataSetConfig.md">Random CSV Data Set plugin</a> is used.
        /// <br/>
        /// <b>Warning:</b> Getting lines in random order has a performance penalty.
        /// <br/>
        /// <b>Warning:</b> When random order is enabled, share mode THREAD_GROUP is not supported.
        /// </summary>
        /// <returns>the dataset for further configuration or usage.</returns>
        public DslCsvDataSet RandomOrder()
            => RandomOrder(true);

        /// <summary>
        /// Same as <see cref="RandomOrder()"/> but allowing to enable or disable it.
        /// <br/>
        /// This is helpful when the resolution is taken at runtime.
        /// </summary>
        /// <param name="enable">specifies to enable or disable the setting. By default, it is set to false.</param>
        /// <returns>the dataset for further configuration or usage.</returns>
        /// <seealso cref="RandomOrder()"/>
        public DslCsvDataSet RandomOrder(bool enable)
        {
            _randomOrder = enable;
            return this;
        }
    }
}
