﻿using System;
using Abstracta.JmeterDsl.Core;
using Abstracta.JmeterDsl.Core.Assertions;
using Abstracta.JmeterDsl.Core.Configs;
using Abstracta.JmeterDsl.Core.Controllers;
using Abstracta.JmeterDsl.Core.Listeners;
using Abstracta.JmeterDsl.Core.PostProcessors;
using Abstracta.JmeterDsl.Core.PreProcessors;
using Abstracta.JmeterDsl.Core.Samplers;
using Abstracta.JmeterDsl.Core.ThreadGroups;
using Abstracta.JmeterDsl.Core.Timers;
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
        /// Builds a new thread group without any thread configuration.
        /// <br/>
        /// This method should be used as starting point for creating complex test thread profiles (like
        /// spike, or incremental tests) in combination with holdFor, rampTo and rampToAndHold <see cref="DslThreadGroup"/> methods.
        /// <br/>
        /// Eg:
        /// <c>
        /// ThreadGroup()
        ///   .RampTo(10, TimeSpan.FromSeconds(10)
        ///   .RampTo(5, TimeSpan.FromSeconds(10))
        ///   .RampToAndHold(20, TimeSpan.FromSeconds(5),TimeSpan.FromSeconds(10))
        ///   .RampTo(0, TimeSpan.FromSeconds(5))
        ///   .Children(...)
        /// </c>
        /// <br/>
        /// For complex thread profiles that can't be mapped to JMeter built-in thread group element, the
        /// DSL uses <a href="https://jmeter-plugins.org/wiki/UltimateThreadGroup/">Ultimate Thread Group plugin</a>
        /// </summary>
        /// <returns>the thread group instance</returns>
        public static DslThreadGroup ThreadGroup() =>
            new DslThreadGroup(null);

        /// <summary>
        /// Same as <see cref="ThreadGroup()"/> but allowing to set a name on the thread group.
        /// <br/>
        /// Setting a proper name allows to properly identify the requests generated in each thread group.
        /// </summary>
        /// <seealso cref="ThreadGroup()"/>
        public static DslThreadGroup ThreadGroup(string name) =>
            new DslThreadGroup(name);

        /// <summary>
        /// Builds a new transaction controller with the given name.
        /// </summary>
        /// <param name="name">specifies the name to identify the transaction.</param>
        /// <param name="children">contains the test elements that will be contained within the transaction.</param>
        /// <returns>the transaction instance.</returns>
        /// <seealso cref="DslTransactionController"/>
        public static DslTransactionController Transaction(string name, params IThreadGroupChild[] children) =>
            new DslTransactionController(name, children);

        /// <summary>
        /// Builds a new simple controller with the given name.
        /// <br/>
        /// Simple controllers are good for defining test plan scopes, without having to create a
        /// transaction (which generates a sample result). For example, to apply configs, assertions,
        /// timers, listeners, post- and pre-processors to only part of the test plan (certain samplers).
        /// <br/>
        /// Additionally, they are a handy way of creating methods that return certain part of a test plan
        /// (inside a simple controller) and then directly inject the method returned controller in an
        /// existing test plan.
        /// </summary>
        /// <param name="children">contains the test elements that will be contained within the controller.</param>
        /// <returns>the controller instance.</returns>
        /// <seealso cref="DslSimpleController"/>
        public static DslSimpleController SimpleController(params IThreadGroupChild[] children) =>
            SimpleController(null, children);

        /// <summary>
        /// Same as <see cref="SimpleController(IThreadGroupChild[])"/> but allowing to set a name.
        /// <br/>
        /// In this scenario, the name has no functional usage during test plan execution, but it can ease
        /// JMX test plan review as to keep sections of the test plan well identified.
        /// </summary>
        /// <seealso cref="SimpleController(IThreadGroupChild[])"/>
        public static DslSimpleController SimpleController(string name, params IThreadGroupChild[] children) =>
            new DslSimpleController(name, children);

        /// <summary>
        /// Builds a Loop Controller that allows to run specific number of times the given children in each
        /// thread group iteration.
        /// <br/>
        /// Eg: if a thread group iterates 3 times and the Loop Controller is configured to 5, then the
        /// children elements will run <c>3*5=15</c> times for each thread.
        /// <br/>
        /// JMeter generates <c>__jm__for__idx</c> variable containing the iteration number (0 indexed),
        /// which can be helpful in some scenarios.
        /// </summary>
        /// <param name="count">specifies the number of times to execute the children elements in each thread group iteration.</param>
        /// <param name="children">contains the test plan elements to execute the given number of times in each thread group iteration.</param>
        /// <returns>the controller instance for further configuration and usage.</returns>
        /// <seealso cref="Core.Controllers.ForLoopController"/>
        public static ForLoopController ForLoopController(int count, params IThreadGroupChild[] children) =>
            ForLoopController(null, count, children);

        /// <summary>
        /// Same as <see cref="ForLoopController(int, IThreadGroupChild[])"/> but allowing to set a name which
        /// defines autogenerated variable created by JMeter containing iteration index.
        /// </summary>
        /// <param name="name">specifies the name to assign to the controller. This variable affects the JMeter autogenerated variable <c>__jm__&lt;controllerName&gt;__idx}</c> which holds the loop iteration number (starting at 0).</param>
        /// <param name="count">specifies the number of times to execute the children elements in each thread group iteration.</param>
        /// <param name="children">contains the test plan elements to execute the given number of times in each thread group iteration.</param>
        /// <returns>the controller instance for further configuration and usage.</returns>
        /// <seealso cref="Core.Controllers.ForLoopController"/>
        /// <seealso cref="ForLoopController(int, IThreadGroupChild[])"/>
        public static ForLoopController ForLoopController(string name, int count, params IThreadGroupChild[] children) =>
            ForLoopController(name, count.ToString(), children);

        /// <summary>
        /// Same as <see cref="ForLoopController(int, IThreadGroupChild[])"/> but allowing to use JMeter
        /// expressions for number of loops.
        /// <br/>
        /// This method allows, for example, to extract from a previous response the number of times to
        /// execute some part of the test plan and use it in forLoop with something like <c>${LOOPS_COUNT}</c>.
        /// </summary>
        /// <param name="count">specifies a JMeter expression which evaluates to a number specifying the number of times to execute the children elements in each thread group iteration.</param>
        /// <param name="children">contains the test plan elements to execute the given number of times in each thread group iteration.</param>
        /// <returns>the controller instance for further configuration and usage.</returns>
        /// <seealso cref="ForLoopController(int, IThreadGroupChild[])"/>
        public static ForLoopController ForLoopController(string count, params IThreadGroupChild[] children) =>
            new ForLoopController(null, count, children);

        /// <summary>
        /// Same as <see cref="ForLoopController(string, IThreadGroupChild[])"/> but allowing to set a name
        /// which defines autogenerated variable created by JMeter containing iteration index.
        /// </summary>
        /// <param name="name">specifies the name to assign to the controller. This variable affects the JMeter autogenerated variable <c>__jm__&lt;controllerName&gt;__idx}</c> which holds the loop iteration number (starting at 0).</param>
        /// <param name="count">specifies a JMeter expression which evaluates to a number specifying the number of times to execute the children elements in each thread group iteration.</param>
        /// <param name="children">contains the test plan elements to execute the given number of times in each thread group iteration.</param>
        /// <returns>the controller instance for further configuration and usage.</returns>
        /// <seealso cref="Core.Controllers.ForLoopController"/>
        /// <seealso cref="ForLoopController(int, IThreadGroupChild[])"/>
        public static ForLoopController ForLoopController(string name, string count, params IThreadGroupChild[] children) =>
            new ForLoopController(name, count, children);

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
        /// Builds a JSR223 Pre Processor which allows including custom logic to modify requests.
        /// <br/>
        /// This preprocessor is very powerful, and lets you alter request parameters, jmeter context and
        /// implement any kind of custom logic that you may think.
        /// </summary>
        /// <param name="script">contains the script to be executed by the preprocessor. By default, this will be
        ///               a groovy script, but you can change it by setting the language property in the
        ///               returned post processor.</param>
        /// <returns>the JSR223 Pre Processor instance</returns>
        /// <seealso cref="DslJsr223PreProcessor"/>
        public static DslJsr223PreProcessor Jsr223PreProcessor(string script) =>
            Jsr223PreProcessor(null, script);

        /// <summary>
        /// Same as <see cref="Jsr223PreProcessor(string)"/> but allowing to set a name on the preprocessor.
        /// <br/>
        /// The name is used as logger name which allows configuring log level, appender, etc., for the
        /// preprocessor.
        /// </summary>
        /// <seealso cref="Jsr223PreProcessor(string)"/>
        public static DslJsr223PreProcessor Jsr223PreProcessor(string name, string script) =>
            new DslJsr223PreProcessor(name, script);

        /// <summary>
        /// Builds a Regex Extractor which allows using regular expressions to extract different parts of a
        /// sample result (request or response).
        /// <br/>
        /// This method provides a simple default implementation with required settings, but more settings
        /// are provided by returned DslRegexExtractor.
        /// <br/>
        /// By default, when regex is not matched, no variable will be created or modified. On the other
        /// hand when the regex matches it will by default store the first capturing group (part of
        /// expression between parenthesis) of the first match for the regular expression.
        /// </summary>
        /// <param name="variableName">is the name of the variable to be used to store the extracted value to.
        /// Additional variables <c>&lt;variableName&gt;_g&lt;groupId&gt;</c> will be created for
        /// each regular expression capturing group (segment of regex between
        /// parenthesis), being the group 0 the entire match of the regex.
        /// <c>&lt;variableName&gt;_g</c> variable contains the number of matched capturing groups
        /// (not counting the group 0).</param>
        /// <param name="regex">regular expression used to extract part of request or response.</param>
        /// <returns>the Regex Extractor which can be used to define additional settings to use when
        /// extracting (like defining match number, template, etc.).</returns>
        /// <seealso cref="DslRegexExtractor"/>
        public static DslRegexExtractor RegexExtractor(string variableName, string regex)
            => new DslRegexExtractor(variableName, regex);

        /// <summary>
        /// Builds a Response Assertion to be able to check that obtained sample result is the expected
        /// one.
        /// <br/>
        /// JMeter by default uses repose codes (eg: 4xx and 5xx HTTP response codes are error codes) to
        /// determine if a request was success or not, but in some cases this might not be enough or
        /// correct. In some cases applications might not behave in this way, for example, they might
        /// return a 200 HTTP status code but with an error message in the body, or the response might be a
        /// success one, but the information contained within the response is not the expected one to
        /// continue executing the test. In such scenarios you can use response assertions to properly
        /// verify your assumptions before continuing with next request in the test plan.
        /// <br/>
        /// By default, response assertion will use the response body of the main sample result (not sub
        /// samples as redirects, or embedded resources) to check the specified criteria (substring match,
        /// entire string equality, contained regex or entire regex match) against.
        /// </summary>
        /// <returns>the created Response Assertion which should be modified to apply the proper criteria.
        /// Check <see cref="DslResponseAssertion"/> for all available options.</returns>
        /// <seealso cref="DslResponseAssertion"/>
        public static DslResponseAssertion ResponseAssertion() =>
            new DslResponseAssertion(null);

        /// <summary>
        /// Same as <see cref="ResponseAssertion()"/> but allowing to set a name on the assertion, which can be
        /// later used to identify assertion results and differentiate it from other assertions.
        /// </summary>
        /// <param name="name">is the name to be assigned to the assertion</param>
        /// <returns>the created Response Assertion which should be modified to apply the proper criteria.
        /// Check <see cref="DslResponseAssertion"/> for all available options.</returns>
        /// <seealso cref="DslResponseAssertion"/>
        public static DslResponseAssertion ResponseAssertion(string name) =>
            new DslResponseAssertion(name);

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

        /// <summary>
        /// Builds a Flow Control Action that pauses the current thread for the given duration.
        /// <br/>
        /// This is an alternative to timers, which eases adding just one pause instead of applying a pause
        /// to all elements in the scope of a timer.
        /// </summary>
        /// <param name="duration">specifies the duration for the pause.</param>
        /// <returns>the test element for usage in a test plan</returns>
        public static DslFlowControlAction ThreadPause(TimeSpan duration) =>
            ThreadPause(duration.TotalMilliseconds.ToString());

        /// <summary>
        /// Same as <see cref="ThreadPause(TimeSpan)"/> but allowing to use JMeter expressions for the
        /// duration.
        /// <br/>
        /// For example, you can set a delay depending on the amount of time taken in last sample with
        /// something like <c>${__groovy(5000 - prev.time)}</c>.
        /// </summary>
        /// <param name="duration">JMeter expression that evaluates to the number of milliseconds to
        /// pause the thread.</param>
        /// <returns>the test element for usage in a test plan</returns>
        public static DslFlowControlAction ThreadPause(string duration) =>
            DslFlowControlAction.PauseThread(duration);

        /// <summary>
        /// Builds a Constant Timer which pauses the thread with for a given duration.
        /// </summary>
        /// <param name="duration">specifies the duration for the timer to wait.</param>
        /// <returns>the timer for usage in test plan.</returns>
        public static DslConstantTimer ConstantTimer(TimeSpan duration) =>
            ConstantTimer(duration.TotalMilliseconds.ToString());

        /// <summary>
        /// Same as <see cref="ConstantTimer(TimeSpan)"/> but allowing to use JMeter expression.
        /// <br/>
        /// For example, you can set a delay depending on the amount of time taken in last sample with
        /// something like <c>${__groovy(5000 - prev.time)}</c>.
        /// </summary>
        /// <param name="duration">JMeter expression that evaluates to the number of milliseconds to
        /// pause the thread.</param>
        /// <returns>the timer for usage in test plan.</returns>
        public static DslConstantTimer ConstantTimer(string duration) =>
            new DslConstantTimer(duration);

        /// <summary>
        /// Builds a Uniform Random Timer which pauses the thread with a random time with uniform
        /// distribution.
        /// <br/>
        /// The timer uses the minimum and maximum durations to define the range of values to be used in
        /// the uniformly distributed selected value. These values differ from the parameters used in
        /// JMeter Uniform Random Timer element to make it simpler for general users to use. The generated
        /// JMeter test element uses as "constant delay offset" the minimum value, and as "maximum random
        /// delay" (maximum - minimum) value.
        /// <br/>
        /// EXAMPLE: wait at least 3 seconds and maximum of 10 seconds
        /// {@code uniformRandomTimer(Duration.ofSeconds(3), Duration.ofSeconds(10))}
        /// </summary>
        /// <param name="minimum">is used to set the constant delay of the Uniform Random Timer.</param>
        /// <param name="maximum">is used to set the maximum time the timer will be paused and will be used to
        ///                obtain the random delay from the result of (maximum - minimum).</param>
        /// <returns>The Uniform Random Timer instance</returns>
        /// <see cref="DslUniformRandomTimer"/>
        public static DslUniformRandomTimer UniformRandomTimer(TimeSpan minimum, TimeSpan maximum) =>
            new DslUniformRandomTimer(minimum, maximum);

        /// <summary>
        /// Builds a Synchronizing Timer that allows synchronizing samples to be sent all at once.
        /// <br/>
        /// This timer is useful when you need to send requests in simultaneous batches, as a way to asure
        /// the system under test gets the requests all at the same time.
        /// </summary>
        /// <returns>the timer for usage in a test plan.</returns>
        /// <see cref="DslSynchronizingTimer"/>
        public static DslSynchronizingTimer SynchronizingTimer() =>
            new DslSynchronizingTimer();

        /// <summary>
        /// Builds a CSV Data Set which allows loading from a CSV file variables to be used in test plan.
        /// <br/>
        /// This allows to store for example in a CSV file one line for each user credentials, and then in
        /// the test plan be able to use all the credentials to test with different users.
        /// <br/>
        /// By default, the CSV data set will read comma separated values, use first row as name of the
        /// generated variables, restart from beginning when csv entries are exhausted and will read a new
        /// line of CSV for each thread and iteration.
        /// <br/>
        /// E.g: If you have a csv with 2 entries and a test plan with two threads, iterating 2 times each,
        /// you might get (since threads run in parallel, the assignment is not deterministic) following
        /// assignment of rows:
        /// <br/>
        /// <pre>
        /// thread 1, row 1
        /// thread 2, row 2
        /// thread 2, row 1
        /// thread 1, row 2
        /// </pre>
        /// </summary>
        /// <param name="csvFile">path to the CSV file to read the data from.</param>
        /// <returns>the CSV Data Set instance for further configuration and usage.</returns>
        /// <seealso cref="DslCsvDataSet"/>
        public static DslCsvDataSet CsvDataSet(string csvFile) =>
            new DslCsvDataSet(csvFile);
    }
}
