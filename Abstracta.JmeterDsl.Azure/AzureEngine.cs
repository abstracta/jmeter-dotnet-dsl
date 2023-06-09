using System;
using Abstracta.JmeterDsl.Core.Engines;

namespace Abstracta.JmeterDsl.Azure
{
    /// <summary>
    /// A <see cref="Core.IDslJmeterEngine"/> which allows running DslTestPlan in Azure Load Testing.
    /// <br/>
    /// To use this engine you need:
    /// <ul>
    /// <li>To create a test resource and resource group in Azure Load Testing.First defined test
    /// resource for the subscription, is used by default.</li>
    /// <li>Register an application in Azure with proper permissions for Azure Load Testing with an
    /// associated secret. Check <a href="https://learn.microsoft.com/en-us/azure/active-directory/develop/howto-create-service-principal-portal"> here</a>
    /// for more details.</li>
    /// </ul>
    /// </summary>
    public class AzureEngine : BaseJmeterEngine<AzureEngine>
    {
        private readonly string _credentials;
        private string _subscriptionId;
        private string _resourceGroupName;
        private string _location;
        private string _testResourceName;
        private string _testName = "jmeter-dotnet-dsl";
        private TimeSpan? _testTimeout;
        private int? _engines;

        /// <summary>
        /// Builds a new AzureEngine from a given string containing tenant id, client id and client secrets
        /// separated by colons.
        /// <br/>
        /// This is just a handy way to specify credentials in a string (eg: environment variable) and
        /// easily create an Azure Engine. For a more explicit way you may use <see cref="AzureEngine(string, string, string)"/>.
        /// </summary>
        /// <param name="credentials">contains tenant id, client id and client secrets separated by colons. Eg:
        /// myTenantId:myClientId:mySecret.
        /// <br/>
        /// Check <a href="https://learn.microsoft.com/en-us/azure/active-directory/develop/howto-create-service-principal-portal">the
        /// Azure guide</a> for instructions on how to register an application with
        /// proper access. Tip: there is no need to specify a redirect uri.
        /// <br/>
        /// The tenantId can easily be retrieved getting subscription info in Azure Portal.
        /// </param>
        public AzureEngine(string credentials)
        {
            _credentials = credentials;
        }

        /// <summary>
        /// This is a more explicit way to create AzureEngine than <see cref="AzureEngine(string)"/>.
        /// <br/>
        /// This is usually preferred when you already have each credential value separated, as is more
        /// explicit and doesn't require encoding into a string.
        /// </summary>
        /// <param name="tenantId">is the tenant id for your subscription. This can easily be retrieved
        /// getting subscription info in Azure Portal</param>
        /// <param name="clientId">this is the id associated to the test that needs to run in Azure. You
        /// should use one application for each JMeter DSL project that uses Azure Load
        /// Testing. This can be retrieved when register an application following steps
        /// detailed in <a href="https://learn.microsoft.com/en-us/azure/active-directory/develop/howto-create-service-principal-portal">this Azure guide</a>.
        /// </param>
        /// <param name="clientSecret">this is a client secret generated for the test to be run in Azure.</param>
        public AzureEngine(string tenantId, string clientId, string clientSecret)
        {
            _credentials = $"{tenantId}:{clientId}:{clientSecret}";
        }

        /// <summary>
        /// Allows specifying the Azure subscription ID to run the tests on.
        /// <br/>
        /// By default, AzureEngine will use any subscription associated to the given tenant. In most of
        /// the scenarios, when you only use one subscription, this behavior is good. But, when you have
        /// multiple subscriptions, it is necessary to specify which subscription from the available ones
        /// you want to use. This method is for those scenarios.
        /// </summary>
        /// <param name="subscriptionId">specifies the Azure subscription identifier to use while running tests.
        /// When not specified, any subscription associated to the tenant will be
        /// used.</param>
        /// <returns>the engine for further configuration or usage.</returns>
        public AzureEngine SubscriptionId(string subscriptionId)
        {
            _subscriptionId = subscriptionId;
            return this;
        }

        /// <summary>
        /// Specifies the name the resource group where tests will be created or updated.
        /// <br/>
        /// You can use Azure resource groups to group different test resources (projects, systems under
        /// test) shared by members of a team.
        /// <br/>
        /// If a resource group exists with the given name, then that group will be used. Otherwise, a new
        /// one will be created. You can use <see cref="Location(string)"/> to specify the location where the
        /// resource group will be created (by default, the first available one will be used, eg: eastus).
        /// </summary>
        /// <param name="resourceGroupName">specifies the name of the resource group to use. If no name is
        /// specified, then the test resource name (<see cref="TestResourceName(string)"/>)
        /// plus "-rg" suffix is used. Eg: jmeter-dotnet-dsl-rg.</param>
        /// <returns>the engine for further configuration or usage.</returns>
        public AzureEngine ResourceGroupName(string resourceGroupName)
        {
            _resourceGroupName = resourceGroupName;
            return this;
        }

        /// <summary>
        /// Specifies the location where to create new resource groups.
        /// </summary>
        /// <param name="location">the Azure location to use when creating new resource groups. If none is
        /// specified, then the first available location will be used (eg: eastus).</param>
        /// <returns>the engine for further configuration or usage.</returns>
        /// <seealso cref="ResourceGroupName(string)"/>
        public AzureEngine Location(string location)
        {
            _location = location;
            return this;
        }

        /// <summary>
        /// Specifies the name of the test resource where tests will be created or updated.
        /// <br/>
        /// You can use Azure test resources to group different tests resources belonging to the same
        /// project or system under test.
        /// <br/>
        /// If a test resource exists with the given name, then that test resources will be used.
        /// Otherwise, a new one will be created.
        /// </summary>
        /// <param name="testResourceName">specifies the name of the test resource. If no name is specified, then
        /// the test name (<see cref="TestName(string)"/>) is used.</param>
        /// <returns>the engine for further configuration or usage.</returns>
        public AzureEngine TestResourceName(string testResourceName)
        {
            _testResourceName = testResourceName;
            return this;
        }

        /// <summary>
        /// Specifies the name of the test to be created or updated.
        /// <br/>
        /// If a test with the given name exists, then the test is updated. Otherwise, a new one is
        /// created.
        /// </summary>
        /// <param name="testName">specifies the name of the test to create or update. If no name is specified,
        /// then jmeter-dotnet-dsl is used by default.</param>
        /// <returns>the engine for further configuration or usage.</returns>
        public AzureEngine TestName(string testName)
        {
            _testName = testName;
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
        /// <param name="duration">to be used as time limit for test execution. If execution takes more than this,
        /// then a JvmException will be thrown by the engine.</param>
        /// <returns>the engine for further configuration or usage.</returns>
        public AzureEngine TestTimeout(TimeSpan duration)
        {
            _testTimeout = duration;
            return this;
        }

        /// <summary>
        /// Specifies the number of JMeter engine instances where the test plan should run.
        /// <br/>
        /// This value directly impact the generated load. For example: if your test plan defines to use a
        /// thread group with 100 users, then using 3 engines will result in 300 parallel users. Azure Load
        /// Testing simply runs the test plan in as many engines specified by this value.
        /// </summary>
        /// <param name="count">specifies the number of JMeter engine instances to run the test plan on. When not
        /// specified it just runs the test plan in 1 engine.</param>
        /// <returns>the engine for further configuration or usage.</returns>
        public AzureEngine Engines(int count)
        {
            _engines = count;
            return this;
        }
    }
}
