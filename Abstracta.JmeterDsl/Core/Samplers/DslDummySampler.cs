using System;

namespace Abstracta.JmeterDsl.Core.Samplers
{
    /// <summary>
    /// Allows using JMeter Dummy Sampler plugin to emulate other samples and ease testing post
    /// processors and other parts of a test plan.
    /// <br/>
    /// By default, this element is set with no request, url, response code=200, response message = OK,
    /// and response time with random value between 50 and 500 milliseconds. Additionally, emulation of
    /// response times (through sleeps) is disabled to speed up testing.
    /// </summary>
    public class DslDummySampler : BaseSampler
    {
        private readonly string _responseBody;
        private bool? _successful;
        private string _responseCode;
        private string _responseMessage;
        private string _responseTime;
        private bool? _simulateResponseTime;
        private string _url;
        private string _requestBody;

        public DslDummySampler(string name, string responseBody)
            : base(name)
        {
            _responseBody = responseBody;
        }

        /// <summary>
        /// Allows generating successful or unsuccessful sample results for this sampler.
        /// </summary>
        /// <param name="successful">when true, generated sample result will be successful, otherwise it will be
        /// marked as failure. When not specified, successful sample results are
        /// generated.</param>
        /// <returns> the sampler for further configuration or usage.</returns>
        public DslDummySampler Successful(bool successful)
        {
            _successful = successful;
            return this;
        }

        /// <summary>
        /// Specifies the response code included in generated sample results.
        /// </summary>
        /// <param name="code">defines the response code included in sample results. When not set, 200 is used.</param>
        /// <returns> the sampler for further configuration or usage.</returns>
        public DslDummySampler ResponseCode(string code)
        {
            _responseCode = code;
            return this;
        }

        /// <summary>
        /// Specifies the response message included in generated sample results.
        /// </summary>
        /// <param name="message">defines the response message included in sample results. When not set, OK is
        /// used.</param>
        /// <returns> the sampler for further configuration or usage.</returns>
        public DslDummySampler ResponseMessage(string message)
        {
            _responseMessage = message;
            return this;
        }

        /// <summary>
        /// Specifies the response time used in generated sample results.
        /// </summary>
        /// <param name="responseTime">defines the response time associated to the sample results. When not set, a
        /// randomly calculated value between 50 and 500 milliseconds is used.</param>
        /// <returns> the sampler for further configuration or usage.</returns>
        public DslDummySampler ResponseTime(TimeSpan responseTime)
        {
            _responseTime = ((long)responseTime.TotalMilliseconds).ToString();
            return this;
        }

        /// <summary>
        /// Same as <see cref="ResponseTime(TimeSpan)"/> but allowing to specify a JMeter expression for
        /// evaluation.
        /// <br/>
        /// This is useful when you want response time to be calculated dynamically. For example,
        /// <code>${__Random(50, 500)}}</code>
        /// </summary>
        /// <param name="responseTime">specifies the JMeter expression to be used to calculate response times,
        /// in milliseconds, for the sampler.</param>
        /// <returns> the sampler for further configuration or usage.</returns>
        /// <seealso cref="ResponseTime(TimeSpan)"/>
        public DslDummySampler ResponseTime(string responseTime)
        {
            _responseTime = responseTime;
            return this;
        }

        /// <summary>
        /// Specifies if used response time should be simulated (the sample will sleep for the given
        /// duration) or not.
        /// <br/>
        /// Having simulation disabled allows for really fast emulation and trial of test plan, which is
        /// very handy when debugging. If you need a more accurate emulation in more advanced cases, like
        /// you don't want to generate too many requests per second, and you want a behavior closer to the
        /// real thing, then consider enabling response time simulation.
        /// </summary>
        /// <param name="simulate">when true enables simulation of response times, when false no wait is done
        /// speeding up test plan execution. By default, simulation is disabled.</param>
        /// <returns> the sampler for further configuration or usage.</returns>
        public DslDummySampler SimulateResponseTime(bool simulate)
        {
            _simulateResponseTime = simulate;
            return this;
        }

        /// <summary>
        /// Specifies the URL used in generated sample results.
        /// <br/>
        /// This might be helpful in scenarios where extractors, pre-processors or other test plan elements
        /// depend on the URL.
        /// </summary>
        /// <param name="url">defines the URL associated to generated sample results. When not set, an empty URL
        /// is used.</param>
        /// <returns> the sampler for further configuration or usage.</returns>
        public DslDummySampler Url(string url)
        {
            _url = url;
            return this;
        }

        /// <summary>
        /// Specifies the request body used in generated sample results.
        /// <br/>
        /// This might be helpful in scenarios where extractors, pre-processors or other test plan elements
        /// depend on the request body.
        /// </summary>
        /// <param name="requestBody">defines the request body associated to generated sample results. When not
        /// set, an empty body is used.</param>
        /// <returns> the sampler for further configuration or usage.</returns>
        public DslDummySampler RequestBody(string requestBody)
        {
            _requestBody = requestBody;
            return this;
        }
    }
}
