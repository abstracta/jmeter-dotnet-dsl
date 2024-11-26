using System;
using System.Collections.Generic;
using Abstracta.JmeterDsl.Core;
using Abstracta.JmeterDsl.Core.Bridge;
using Abstracta.JmeterDsl.Core.Engines;

namespace Abstracta.JmeterDsl.BlazeMeter
{
    /// <summary>
    /// A <see cref="IDslJmeterEngine"/> which allows running DslTestPlan in BlazeMeter.
    /// </summary>
    public class BlazeMeterEngine : BaseJmeterEngine<BlazeMeterEngine>
    {
        private readonly string _authToken;
        private string _testName = "jmeter-dotnet-dsl";
        private long? _projectId;
        private TimeSpan? _testTimeout;
        private TimeSpan? _availableDataTimeout;
        private int? _totalUsers;
        private TimeSpan? _rampUp;
        private int? _iterations;
        private TimeSpan? _holdFor;
        private int? _threadsPerEngine;
        private bool? _useDebugRun;

        private readonly List<DslLocation> __propsList = new List<DslLocation>();

        /// <summary>
        /// Builds a new instance of BlazeMeterEngine with provided authentication token.
        /// </summary>
        /// <param name="authToken">is the authentication token to be used to access BlazeMeter API.
        /// <br/>
        /// It follows the following format: &lt;Key ID&gt;:&lt;Key Secret&gt;.
        /// <br/>
        /// Check <a href="https://guide.blazemeter.com/hc/en-us/articles/115002213289-BlazeMeter-API-keys-">BlazeMeter
        /// API keys</a> for instructions on how to generate them.
        /// </param>
        public BlazeMeterEngine(string authToken)
        {
            _authToken = authToken;
        }

        /// <summary>
        /// Sets the name of the BlazeMeter test to use.
        /// <br/>
        /// BlazeMeterEngine will search for a test with the given name in the given project (Check
        /// <see cref="ProjectId(long)"/>) and if one exists, it will update it and use it to run the provided
        /// test plan. If a test with the given name does not exist, then it will create a new one to run
        /// the given test plan.
        /// <br/>
        /// When not specified, the test name defaults to "jmeter-dotnet-dsl".
        /// </summary>
        /// <param name="testName">specifies the name of the test to update or create in BlazeMeter.</param>
        /// <returns>the engine for further configuration or usage.</returns>
        public BlazeMeterEngine TestName(string testName)
        {
            _testName = testName;
            return this;
        }

        /// <summary>
        /// Specifies the ID of the BlazeMeter project where to run the test.
        /// <br/>
        /// You can get the ID of the project by selecting a given project in BlazeMeter and getting the
        /// number right after "/projects" in the URL.
        /// <br/>
        /// When no project ID is specified, then the default one for the user (associated to the given
        /// authentication token) is used.
        /// </summary>
        /// <param name="projectId">is the ID of the project to be used to run the test.</param>
        /// <returns>the engine for further configuration or usage.</returns>
        public BlazeMeterEngine ProjectId(long projectId)
        {
            _projectId = projectId;
            return this;
        }

        /// <summary>
        /// Specifies a timeout for the entire test execution.
        /// <br/>
        /// If the timeout is reached then the test run will throw a JvmException.
        /// <br/>
        /// It is strongly advised to set this timeout properly in each run, according to the expected test
        /// execution time plus some additional margin (to consider for additional delays in BlazeMeter
        /// test setup and teardown).
        /// <br/>
        /// This timeout exists to avoid any potential problem with BlazeMeter execution not detected by
        /// the client, and avoid keeping the test indefinitely running until is interrupted by a user.
        /// This is specially annoying when running tests in automated fashion, for example in CI/CD.
        /// <br/>
        /// When not specified, the default timeout will is set to 1 hour.
        /// </summary>
        /// <param name="testTimeout">to be used as time limit for test execution. If execution takes more than
        /// this, then a JvmException will be thrown by the engine.</param>
        /// <returns>the engine for further configuration or usage.</returns>
        public BlazeMeterEngine TestTimeout(TimeSpan testTimeout)
        {
            _testTimeout = testTimeout;
            return this;
        }

        /// <summary>
        /// Specifies a timeout for waiting for test data (metrics) to be available in BlazeMeter.
        /// <br/>
        /// After a test is marked as ENDED in BlazeMeter, it may take a few seconds for the associated
        /// final metrics to be available. In some cases, the test is marked as ENDED by BlazeMeter, but
        /// the data is never available. This usually happens when there is some problem running the test
        /// (for example some internal problem with BlazeMeter engine, some missing jmeter plugin, or some
        /// other jmeter error). This timeout makes sure that tests properly fail (throwing a
        /// JvmException) when they are marked as ENDED and no data is available after the given
        /// timeout, and avoids unnecessary wait for test execution timeout.
        /// <br/>
        /// Usually this timeout should not be necessary to change, but the API provides such method in
        /// case you need to tune such setting.
        /// <br/>
        /// When not specified, this value will default to 30 seconds.
        /// </summary>
        /// <param name="availableDataTimeout">to wait for available data after a test ends, before throwing a JvmException</param>
        /// <returns>the engine for further configuration or usage.</returns>
        public BlazeMeterEngine AvailableDataTimeout(TimeSpan availableDataTimeout)
        {
            _availableDataTimeout = availableDataTimeout;
            return this;
        }

        /// <summary>
        /// Specifies the number of virtual users to use when running the test.
        /// <br/>
        /// This value overwrites any value specified in JMeter test plans thread groups.
        /// <br/>
        /// When no configuration is given for TotalUsers, RampUpFor, Iterations or HoldFor, then
        /// configuration will be taken from the first default thread group found in the test plan.
        /// Otherwise, when no totalUsers is specified, 1 total user for will be used.
        /// </summary>
        /// <param name="totalUsers">number of virtual users to run the test with.</param>
        /// <returns>the engine for further configuration or usage.</returns>
        public BlazeMeterEngine TotalUsers(int totalUsers)
        {
            _totalUsers = totalUsers;
            return this;
        }

        /// <summary>
        /// Sets the duration of time taken to start the specified total users.
        /// <br/>
        /// For example if TotalUsers is set to 10, RampUp is 1 minute and HoldFor is 10 minutes, it means
        /// that it will take 1 minute to start the 10 users (starting them in a linear fashion: 1 user
        /// every 6 seconds), and then continue executing the test with the 10 users for 10 additional
        /// minutes.
        /// <br/>
        /// This value overwrites any value specified in JMeter test plans thread groups.
        /// <br/>
        /// Take into consideration that BlazeMeter does not support specifying this value in units more
        /// granular than minutes, so, if you use a finer grain duration, it will be rounded up to minutes
        /// (eg: if you specify 61 seconds, this will be translated into 2 minutes).
        /// <br/>
        /// When no configuration is given for TotalUsers, RampUpFor, Iterations or HoldFor, then
        /// configuration will be taken from the first default thread group found in the test plan.
        /// Otherwise, when no ramp up is specified, 0 ramp up will be used.
        /// </summary>
        /// <param name="rampUp">duration that BlazeMeter will take to spin up all the virtual users.</param>
        /// <returns>the engine for further configuration or usage.</returns>
        public BlazeMeterEngine RampUpFor(TimeSpan rampUp)
        {
            _rampUp = rampUp;
            return this;
        }

        /// <summary>
        /// Specifies the number of iterations each virtual user will execute.
        /// <br/>
        /// If both Iterations and HoldFor are specified, then iterations are ignored and only HoldFor is
        /// taken into consideration.
        /// <br/>
        /// When neither Iterations and HoldFor are specified, then the last test run configuration is
        /// used, or the criteria specified in the JMeter test plan if no previous test run exists.
        /// <br/>
        /// When no configuration is given for TotalUsers, RampUpFor, Iterations or HoldFor, then
        /// configuration will be taken from the first default thread group found in the test plan.
        /// Otherwise, when no iterations are specified, infinite iterations will be used.
        /// </summary>
        /// <param name="iterations">for each virtual users to execute.</param>
        /// <returns>the engine for further configuration or usage.</returns>
        public BlazeMeterEngine Iterations(int iterations)
        {
            _iterations = iterations;
            return this;
        }

        /// <summary>
        /// Specifies the duration of time to keep the virtual users running, after the rampUp period.
        /// <br/>
        /// If both Iterations and HoldFor are specified, then Iterations are ignored and only HoldFor is
        /// taken into consideration.
        /// <br/>
        /// When neither Iterations and HoldFor are specified, then the last test run configuration is
        /// used, or the criteria specified in the JMeter test plan if no previous test run exists.
        /// <br/>
        /// Take into consideration that BlazeMeter does not support specifying this value in units more
        /// granular than minutes, so, if you use a finer grain duration, it will be rounded up to minutes
        /// (eg: if you specify 61 seconds, this will be translated into 2 minutes).
        /// <br/>
        /// When no configuration is given for TotalUsers, RampUpFor, Iterations or HoldFor, then
        /// configuration will be taken from the first default thread group found in the test plan.
        /// Otherwise, when no hold for or iterations are specified, 10 seconds hold for will be used.
        /// </summary>
        /// <param name="holdFor">duration to keep virtual users running after the RampUp period.</param>
        /// <returns>the engine for further configuration or usage.</returns>
        public BlazeMeterEngine HoldFor(TimeSpan holdFor)
        {
            _holdFor = holdFor;
            return this;
        }

        /// <summary>
        /// Specifies the number of threads/virtual users to use per BlazeMeter engine (host or
        /// container).
        /// <br/>
        /// It is always important to use as less resources (which reduces costs) as possible to generate
        /// the required load for the test. Too few resources might lead to misguiding results, since the
        /// instances/engines running might be saturating and not properly imposing the expected load upon
        /// the system under test. Too many resources might lead to unnecessary expenses (wasted money).
        /// <br/>
        /// This setting, in conjunction with TotalUsers, determines the number of engines BlazeMeter will
        /// use to run the test. For example, if you specify TotalUsers to 500 and 100 ThreadsPerEngine,
        /// then 5 engines will be used to run the test.
        /// <br/>
        /// It is important to set this value appropriately, since different test plans may impose
        /// different load in BlazeMeter engines. This in turns ends up defining different limit of number
        /// of virtual users per engine that a test run requires to properly measure the performance of the
        /// system under test. This process is usually referred as "calibration" and you can read more
        /// about it <a href="https://guide.blazemeter.com/hc/en-us/articles/360001456978-Calibrating-a-JMeter-Test">here</a>.
        /// <br/>
        /// When not specified, the value of the last test run will be used, or the default one for your
        /// BlazeMeter billing plan if no previous test run exists.
        /// </summary>
        /// <param name="threadsPerEngine">the number of threads/virtual users to execute per BlazeMeter engine.</param>
        /// <returns>the engine for further configuration or usage.</returns>
        public BlazeMeterEngine ThreadsPerEngine(int threadsPerEngine)
        {
            _threadsPerEngine = threadsPerEngine;
            return this;
        }

        /// <summary>
        /// Allows specifying BlazeMeter locations where the test plan will run.
        /// <br/>
        /// <see cref="BlazeMeterLocation"/> for a list of known public locations. You can also use a custom private location
        /// like <pre>harbor-5b0323b3c648be3b4c7b23c8</pre> or <pre>My Location</pre>.
        /// <br/>
        /// Use this method multiple times to specify several locations to generate load from multiple
        /// locations in parallel.
        /// <br/>
        /// E.g:
        /// <pre>{@code
        ///  TestPlan(
        ///    ...
        ///  ).RunIn(new BlazeMeterEngine(bzToken)
        ///    // this scenario will run 50% of the users in GCP us-east1 and the rest in us-west1
        ///    .Location(BlazeMeterLocation.GCP_US_EAST_1, 0.5)
        ///    .Location(BlazeMeterLocation.GCP_US_WEST_1, 0.5)
        ///  );
        /// }</pre>
        /// <br/>
        /// When no location is specified, then the default one will be used.
        /// </summary>
        /// <param name="location">specifies a location where to run test plans.</param>
        /// <param name="weight">specifies the weight of this location over others. For instance, if you have
        /// two locations one with weight 1 and the other 2, then the first one will get
        /// 1/(2+1)=33% of TotalUsers and the other will get the rest of the users. In
        /// general is easier to think in terms of percentages, for example for the same
        /// sample set 33 and 67 as weights.</param>
        /// <returns>the engine for further configuration or usage.</returns>
        /// <see cref="BlazeMeterLocation"/>
        public BlazeMeterEngine Location(string location, int weight)
        {
            __propsList.Add(new DslLocation(location, weight));
            return this;
        }

        /// <summary>
        /// Specifies that the test run will use BlazeMeter debug run feature, not consuming credits but
        /// limited up to 10 threads and 5 minutes or 100 iterations.
        /// </summary>
        /// <returns>the engine for further configuration or usage.</returns>
        public BlazeMeterEngine UseDebugRun() =>
            UseDebugRun(true);

        /// <summary>
        /// Same as <see cref="UseDebugRun()"/> but allowing to enable or disable the settign.
        /// <br/>
        /// This is helpful when the resolution is taken at runtime.
        /// </summary>
        /// <param name="enable">enable specifies to enable or disable the setting. By default, it is set to false.</param>
        /// <returns>the engine for further configuration or usage.</returns>
        /// <seealso cref="UseDebugRun()"/>
        public BlazeMeterEngine UseDebugRun(bool enable)
        {
            _useDebugRun = enable;
            return this;
        }

        internal class DslLocation : IDslProperty
        {

            internal readonly string _location;
            internal readonly int _weight;

            public DslLocation(string location, int weight)
            {
                _location = location;
                _weight = weight;
            }
        }
    }
}
