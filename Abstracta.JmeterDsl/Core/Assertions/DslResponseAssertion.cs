using Abstracta.JmeterDsl.Core.TestElements;

namespace Abstracta.JmeterDsl.Core.Assertions
{
    /// <summary>
    /// Allows marking a request result as success or failure by a specific result field value.
    /// </summary>
    public class DslResponseAssertion : DslScopedTestElement<DslResponseAssertion>
    {
        private TargetField _fieldToTest = TargetField.ResponseBody;
        private bool _ignoreStatus;
        private string[] _containsSubstrings;
        private string[] _equalsToStrings;
        private string[] _containsRegexes;
        private string[] _matchesRegexes;
        private bool _invertCheck;
        private bool _anyMatch;

        public DslResponseAssertion(string name)
            : base(name)
        {
        }

        /// <summary>
        /// Identifies a particular field to apply the assertion to.
        /// </summary>
        public enum TargetField
        {
            /// <summary>
            /// Applies the assertion to the response body.
            /// </summary>
            ResponseBody,

            /// <summary>
            /// Applies the assertion to the text obtained through <a href="http://tika.apache.org/1.2/formats.html">Apache Tika</a>
            /// from the response body (which might be a pdf, excel, etc.).
            /// </summary>
            ResponseBodyAsDocument,

            /// <summary>
            /// Applies the assertion to the response code (eg: the HTTP response code, like 200).
            /// </summary>
            ResponseCode,

            /// <summary>
            /// Applies the assertion to the response message (eg: the HTTP response message, like OK).
            /// </summary>
            ResponseMessage,

            /// <summary>
            /// Applies the assertion to the response headers. Response headers is a string with headers
            /// separated by new lines and names and values separated by colons.
            /// </summary>
            ResponseHeaders,

            /// <summary>
            /// Applies the assertion to the set of request headers. Request headers is a string with headers
            /// separated by new lines and names and values separated by colons.
            /// </summary>
            RequestHeaders,

            /// <summary>
            /// Applies the assertion to the request URL.
            /// </summary>
            RequestUrl,

            /// <summary>
            /// Applies the assertion to the request body.
            /// </summary>
            RequestBody,
        }

        /// <summary>
        /// Specifies what field to apply the assertion to.
        /// <br/>
        /// When not specified it will apply the given assertion to the response body.
        /// </summary>
        /// <param name="fieldToTest">specifies the field to apply the assertion to.</param>
        /// <returns>the response assertion for further configuration or usage.</returns>
        /// <seealso cref="TargetField"/>
        public DslResponseAssertion FieldToTest(TargetField fieldToTest)
        {
            _fieldToTest = fieldToTest;
            return this;
        }

        /// <summary>
        /// Specifies that any previously status set to the request should be ignored, and request should
        /// be marked as success by default.
        /// <br/>
        /// This allows overriding the default behavior provided by JMeter when marking requests as failed
        /// (eg: HTTP status codes like 4xx or 5xx). This is particularly useful when tested application
        /// returns an unsuccessful response (eg: 400) but you want to consider some of those cases still
        /// as successful using a different criteria to determine when they are actually a failure (an
        /// unexpected response).
        /// <br/>
        /// Take into consideration that if you specify multiple response assertions to the same sampler,
        /// then if this flag is enabled, any previous assertion result in same sampler will be ignored
        /// (marked as success). So, consider setting this flag in first response assertion only.
        /// </summary>
        /// <returns>the response assertion for further configuration or usage.</returns>
        public DslResponseAssertion IgnoreStatus() =>
            IgnoreStatus(true);

        /// <summary>
        /// Same as <see cref="IgnoreStatus()"/> but allowing to enable or disable it.
        /// <br/>
        /// This is helpful when the resolution is taken at runtime.
        /// </summary>
        /// <param name="enable">specifies to enable or disable the setting. By default, it is set to false.</param>
        /// <returns>the response assertion for further configuration or usage.</returns>
        /// <seealso cref="IgnoreStatus()"/>
        public DslResponseAssertion IgnoreStatus(bool enable)
        {
            _ignoreStatus = enable;
            return this;
        }

        /// <summary>
        /// Checks if the specified <see cref="FieldToTest(TargetField)"/> contains the given substrings.
        /// <br/>
        /// By default, the main sample (not sub samples) response body will be checked, and all supplied
        /// substrings must be contained. Review other methods in this class if you need to check
        /// substrings but in some other ways (eg: in response headers, any match is enough, or none of
        /// specified substrings should be contained).
        /// </summary>
        /// <param name="substrings">list of strings to be searched in the given field to test (by default
        /// response body).</param>
        /// <returns>the response assertion for further configuration or usage.</returns>
        public DslResponseAssertion ContainsSubstrings(params string[] substrings)
        {
            _containsSubstrings = substrings;
            return this;
        }

        /// <summary>
        /// Compares the configured <see cref="FieldToTest(TargetField)"/> to the given strings for equality.
        /// <br/>
        /// By default, the main sample (not sub samples) response body will be checked, and all supplied
        /// strings must be equal to the body (in default setting only makes sense to specify one string).
        /// Review other methods in this class if you need to check equality to entire strings but in some
        /// other ways (eg: in response headers, any match is enough, or none of specified strings should
        /// be equal to the field value).
        /// </summary>
        /// <param name="strings">list of strings to be compared against the given field to test (by default
        ///                response body).</param>
        /// <returns>the response assertion for further configuration or usage.</returns>
        public DslResponseAssertion EqualsToStrings(params string[] strings)
        {
            _equalsToStrings = strings;
            return this;
        }

        /// <summary>
        /// Checks if the configured <see cref="FieldToTest(TargetField)"/> contains matches for given regular
        /// expressions.
        /// <br/>
        /// By default, the main sample (not sub samples) response body will be checked, and all supplied
        /// regular expressions must contain a match in the body. Review other methods in this class if you
        /// need to check regular expressions matches are contained but in some other ways (eg: in response
        /// headers, any regex match is enough, or none of specified regex should be contained in the field
        /// value).
        /// <br/>
        /// By default, regular expressions evaluate in multi-line mode, which means that '.' does not
        /// match new lines, '^' matches start of lines and '$' matches end of lines. To use single-line
        /// mode prefix '(?s)' to the regular expressions. Regular expressions are also by default
        /// case-sensitive, which can be changed to insensitive by adding '(?i)' to the regex.
        /// </summary>
        /// <param name="regexes">list of regular expressions to search for matches in the field to test (by
        ///                default response body).</param>
        /// <returns>the response assertion for further configuration or usage.</returns>
        public DslResponseAssertion ContainsRegexes(params string[] regexes)
        {
            _containsRegexes = regexes;
            return this;
        }

        /// <summary>
        /// Checks if the configured <see cref="FieldToTest(TargetField)"/> matches (completely, and not just
        /// part of it) given regular expressions.
        /// <br/>
        /// By default, the main sample (not sub samples) response body will be checked, and all supplied
        /// regular expressions must match the entire body. Review other methods in this class if you need
        /// to check regular expressions matches but in some other ways (eg: in response headers, any regex
        /// match is enough, or none of specified regex should be matched with the field value).
        /// <br/>
        /// By default, regular expressions evaluate in multi-line mode, which means that '.' does not
        /// match new lines, '^' matches start of lines and '$' matches end of lines. To use single-line
        /// mode prefix '(?s)' to the regular expressions. Regular expressions are also by default
        /// case-sensitive, which can be changed to insensitive by adding '(?i)' to the regex.
        /// </summary>
        /// <param name="regexes">list of regular expressions the field to test (by default response body) must
        ///                match.</param>
        /// <returns>the response assertion for further configuration or usage.</returns>
        public DslResponseAssertion MatchesRegexes(params string[] regexes)
        {
            _matchesRegexes = regexes;
            return this;
        }

        /// <summary>
        /// Allows inverting/negating each of the checks applied by the assertion.
        /// <br/>
        /// This is the same as the "Not" option in Response Assertion in JMeter GUI.
        /// <br/>
        /// It is important to note that the inversion of the check happens at each check and not to the
        /// final result. Eg:
        /// <c>ResponseAssertion().ContainsSubstrings("error", "failure").InvertCheck()</c>
        /// <br/>
        /// Will check that the response does not contain "error" and does not contain "failure". You can
        /// think it as <c>!(ContainsSubstring("error")) &amp;&amp; !(ContainsSubstring("failure"))</c>.
        /// <br/>
        /// Similar logic applies when using in combination with anyMatch method. Eg:
        /// <c>ResponseAssertion().ContainsSubstrings("error", "failure").InvertCheck().MatchAny()</c>
        /// <br/>
        /// Will check that response does not contain both "error" and "failure" at the same time. This is
        /// analogous to <c>!(ContainsSubstring("error")) || !(ContainsSubstring("failure))</c>, which is
        /// equivalent to <c>!(ContainsSubstring("error") &amp;&amp; ContainsSubstring("failure))</c>.
        /// <br/>
        /// Keep in mind that order of invocations of methods in response assertion is irrelevant (so
        /// <c>InvertCheck().MatchAny()</c> gets the same result as <c>MatchAny().InvertCheck()</c>).
        /// </summary>
        /// <returns>the response assertion for further configuration or usage.</returns>
        public DslResponseAssertion InvertCheck() =>
            InvertCheck(true);

        /// <summary>
        /// Same as <see cref="InvertCheck()"/> but allowing to enable or disable it.
        /// <br/>
        /// This is helpful when the resolution is taken at runtime.
        /// </summary>
        /// <param name="enable">specifies to enable or disable the setting. By default, it is set to false.</param>
        /// <returns>the response assertion for further configuration or usage.</returns>
        /// <seealso cref="InvertCheck()"/>
        public DslResponseAssertion InvertCheck(bool enable)
        {
            _invertCheck = enable;
            return this;
        }

        /// <summary>
        /// Specifies that if any check matches then the response assertion is satisfied.
        /// <br/>
        /// This is the same as the "Or" option in Response Assertion in JMeter GUI.
        /// <br/>
        /// By default, when you use something like this:
        /// <c>ResponseAssertion().ContainsSubstrings("success", "OK")</c>
        /// <br/>
        /// The response assertion will be success when both "success" and "OK" sub strings appear in
        /// response body (if one or both don't appear, then it fails). You can think of it like
        /// <c>ContainsSubstring("success") &amp;&amp; ContainsSubstring("OK")</c>.
        /// <br/>
        /// If you want to check that any of them matches then use anyMatch, like this:
        /// <c>ResponseAssertion().ContainsSubstrings("success", "OK").AnyMatch()</c>
        /// <br/>
        /// Which you can interpret as <c>ContainsSubstring("success") || ContainsSubstring("OK")</c>.
        /// </summary>
        /// <returns>the response assertion for further configuration or usage.</returns>
        public DslResponseAssertion AnyMatch() =>
            AnyMatch(true);

        /// <summary>
        /// Same as <see cref="AnyMatch()"/> but allowing to enable or disable it.
        /// <br/>
        /// This is helpful when the resolution is taken at runtime.
        /// </summary>
        /// <param name="enable">specifies to enable or disable the setting. By default, it is set to false.</param>
        /// <returns>the response assertion for further configuration or usage.</returns>
        /// <seealso cref="AnyMatch()"/>
        public DslResponseAssertion AnyMatch(bool enable)
        {
            _anyMatch = enable;
            return this;
        }
    }
}