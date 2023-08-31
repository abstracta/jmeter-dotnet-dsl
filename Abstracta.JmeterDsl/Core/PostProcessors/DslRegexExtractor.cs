namespace Abstracta.JmeterDsl.Core.PostProcessors
{
    /// <summary>
    /// Allows extracting part of a request or response using regular expressions to store into a
    /// variable.
    /// <br/>
    /// By default, the regular extractor is configured to extract from the main sample (does not include
    /// sub samples) response body the first capturing group (part of regular expression that is inside
    /// of parenthesis) of the first match of the regex. If no match is found, then the variable will not
    /// be created or modified.
    /// </summary>
    public class DslRegexExtractor : DslVariableExtractor<DslRegexExtractor>
    {
        private readonly string _regex;
        private string _template;
        private DslTargetField? _fieldToCheck;

        public DslRegexExtractor(string varName, string regex)
            : base(null, varName)
        {
            _regex = regex;
        }

        public enum DslTargetField
        {
            /// <summary>
            /// Applies the regular extractor to the plain string of the response body.
            /// </summary>
            ResponseBody,

            /// <summary>
            /// Applies the regular extractor to the response body replacing all HTML escape codes.
            /// </summary>
            ResponseBodyUnescaped,

            /// <summary>
            /// Applies the regular extractor to the string representation obtained from parsing the response
            /// body with <a href="http://tika.apache.org/1.2/formats.html">Apache Tika</a>.
            /// </summary>
            ResponseBodyAsDocument,

            /// <summary>
            /// Applies the regular extractor to response headers. Response headers is a string with headers
            /// separated by new lines and names and values separated by colons.
            /// </summary>
            ResponseHeaders,

            /// <summary>
            /// Applies the regular extractor to request headers. Request headers is a string with headers
            /// separated by new lines and names and values separated by colons.
            /// </summary>
            RequestHeaders,

            /// <summary>
            /// Applies the regular extractor to the request URL.
            /// </summary>
            RequestUrl,

            /// <summary>
            /// Applies the regular extractor to response code.
            /// </summary>
            ResponseCode,

            /// <summary>
            /// Applies the regular extractor to response message.
            /// </summary>
            ResponseMessage,
        }

        /// <summary>
        /// Sets the match number to be extracted.
        /// <br/>
        /// For example, if a response looks like this:
        /// <c>user=test&amp;user=tester</c>
        /// and you use <c>user=([^&amp;]+)</c> as regular expression, first match (1) would extract
        /// <c>test</c> and second match (2) would extract <c>tester</c>.
        /// <br/>
        /// When not specified, the first match will be used. When 0 is specified, a random match will be
        /// used. When negative, all the matches are extracted to variables with name
        /// <c>&lt;variableName&gt;_&lt;matchNumber&gt;</c>, the number of matches is stored in
        /// <c>&lt;variableName&gt;_matchNr</c>, and default value is assigned to <c>&lt;variableName&gt;</c>.
        /// </summary>
        /// <param name="matchNumber">specifies the match number to use.</param>
        /// <returns>the extractor for further configuration or usage.</returns>
        public DslRegexExtractor MatchNumber(int matchNumber)
        {
            _matchNumber = matchNumber;
            return this;
        }

        /// <summary>
        /// Specifies the final string to store in the JMeter Variable.
        /// <br/>
        /// The string may contain capturing groups (regular expression segments between parenthesis)
        /// references by using <c>$&lt;groupId&gt;$</c> expressions (eg: <c>$1$</c> for first group). Check
        /// <a href="https://jmeter.apache.org/usermanual/component_reference.html#Regular_Expression_Extractor">JMeter
        /// Regular Expression Extractor documentation</a> for more details.
        /// <br/>
        /// For example, if a response looks like this:
        /// <c>email=tester@abstracta.us</c>
        /// And you use <c>user=([^&amp;]+)</c> as regular expression. Then <c>$1$-$2$</c> will result in
        /// storing in the specified JMeter variable the value <c>tester-abstracta</c>.
        /// <br/>
        /// When not specified <c>$1$</c> will be used.
        /// </summary>
        /// <param name="template">specifies template to use for storing in the JMeter variable.</param>
        /// <returns>the extractor for further configuration or usage.</returns>
        public DslRegexExtractor Template(string template)
        {
            _template = template;
            return this;
        }

        /// <summary>
        /// Sets the default value to be stored in the JMeter variable when the regex does not match.
        /// <br/>
        /// When match number is negative then the value is always assigned to the variable name.
        /// </summary>
        /// A common pattern is to specify this value to a known value (e.g.:
        /// &lt;VAR&gt;_EXTRACTION_FAILURE) and then add some assertion on the variable to mark request as
        /// failure when the match doesn't work.
        /// <br/>
        /// When not specified then the variable will not be set if no match is found.
        /// <param name="defaultValue">specifies the default value to be used.</param>
        /// <returns>the extractor for further configuration or usage.</returns>
        public DslRegexExtractor DefaultValue(string defaultValue)
        {
            _defaultValue = defaultValue;
            return this;
        }

        /// <summary>
        /// Allows specifying what part of request or response to apply the regular extractor to.
        /// <br/>
        /// When not specified then the regular extractor will be applied to the response body.
        /// </summary>
        /// <param name="fieldToCheck">field to apply the regular extractor to.</param>
        /// <returns>the extractor for further configuration or usage.</returns>
        /// <seealso cref="DslTargetField"/>
        public DslRegexExtractor FieldToCheck(DslTargetField fieldToCheck)
        {
            _fieldToCheck = fieldToCheck;
            return this;
        }
    }
}