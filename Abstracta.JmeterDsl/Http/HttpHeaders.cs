using System.Collections.Generic;
using System.Net.Http.Headers;
using Abstracta.JmeterDsl.Core.TestElements;

namespace Abstracta.JmeterDsl.Http
{
    /// <summary>
    /// Allows specifying HTTP headers (through an underlying JMeter HttpHeaderManager) to be used by
    /// HTTP samplers.
    /// <br/>
    /// This test element can be added at different levels (in the same way as HTTPHeaderManager) of a
    /// test plan affecting all samplers in the scope were is added.For example if httpHeaders is
    /// specified at test plan, then all headers will apply to http samplers; if it is specified on
    /// thread group, then only samplers on that thread group would be affected; if specified as a child
    /// of a sampler, only the particular sampler will include such headers.Also take into consideration
    /// that headers specified at lower scope will overwrite ones specified at higher scope (eg: sampler
    /// child headers will overwrite test plan headers).
    /// </summary>
    public class HttpHeaders : BaseTestElement, IMultiLevelTestElement
    {
        private readonly Dictionary<string, string> _headers = new Dictionary<string, string>();

        public HttpHeaders()
            : base(null)
        {
        }

        /// <summary>
        /// Allows to set an HTTP header to be used by HTTP samplers.
        /// <br/>
        /// To specify multiple headers just invoke this method several times with the different header
        /// names and values.
        /// </summary>
        /// <param name="name">specifies name of the HTTP header.</param>
        /// <param name="value">specifies value of the HTTP header.</param>
        /// <returns>the config element for further configuration or usage.</returns>
        public HttpHeaders Header(string name, string value)
        {
            _headers[name] = value;
            return this;
        }

        /// <summary>
        /// Allows to easily specify the Content-Type HTTP header.
        /// </summary>
        /// <param name="value">value to use as Content-Type header.</param>
        /// <returns>the config element for further configuration or usage.</returns>
        public HttpHeaders ContentType(MediaTypeHeaderValue value)
        {
            _headers["Content-Type"] = value.ToString();
            return this;
        }
    }
}
