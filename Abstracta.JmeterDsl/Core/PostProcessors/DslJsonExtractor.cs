namespace Abstracta.JmeterDsl.Core.PostProcessors
{
    /// <summary>
    /// Allows extracting part of a JSON response using JMESPath or JSONPath to store into a variable.
    /// <br/>
    /// By default, the extractor is configured to use JMESPath and to extract from the main sample (does
    /// not include sub samples) response body the first match of the JMESPath. If no match is found,
    /// then variable will be assigned empty string.
    /// </summary>
    public class DslJsonExtractor : DslVariableExtractor<DslJsonExtractor>
    {
        private readonly string _query;
        private DslJsonQueryLanguage? _queryLanguage;

        public DslJsonExtractor(string varName, string query)
            : base(null, varName)
        {
            _query = query;
        }

        /// <summary>
        /// Specifies the query language used to extract from JSON.
        /// </summary>
        public enum DslJsonQueryLanguage
        {
            /// <summary>
            /// Specifies to use JMESPath.
            /// <br/>
            /// Check <a href="https://jmespath.org/">JMESPath site</a> for more details.
            /// </summary>
            JmesPath,

            /// <summary>
            /// Specifies to use JSONPath. You can check
            /// <a href="https://jmeter.apache.org/usermanual/component_reference.html#JSON_Extractor">JMeter
            /// JSON Extractor documentation</a> for documentation on JMeter implementation of JSON Path.
            /// </summary>
            JsonPath,
        }

        /// <summary>
        /// Sets the match number to be extracted.
        /// <br/>
        /// For example, if a response looks like this: <c>[{"name":"test"},{"name":"tester"}]</c>
        /// and you use <c>[].name</c> as JMESPath, first match (1) would extract <c>test</c> and second
        /// match (2) would extract <c>tester</c>.
        /// <br/>
        /// When not specified, the first match will be used. When 0 is specified, a random match will be
        /// used. When negative, all the matches are extracted to variables with name
        /// <c>&lt;variableName&gt;_&lt;matchNumber&gt;</c>, the number of matches is stored in
        /// <c>&lt;variableName&gt;_matchNr</c>, and default value is assigned to <c>&lt;variableName&gt;</c>.
        /// </summary>
        /// <param name="matchNumber">specifies the match number to use.</param>
        /// <returns>the extractor for further configuration and usage.</returns>
        public DslJsonExtractor MatchNumber(int matchNumber)
        {
            _matchNumber = matchNumber;
            return this;
        }

        /// <summary>
        /// Sets the default value to be stored in the JMeter variable when no match is found.
        /// <br/>
        /// When match number is negative then the value is always assigned to the variable name.
        /// <br/>
        /// A common pattern is to specify this value to a known value (e.g.:
        /// &lt;VAR&gt;_EXTRACTION_FAILURE) and then add some assertion on the variable to mark request as
        /// failure when the match doesn't work.
        /// <br/>
        /// When not specified, then the variable will be assigned to empty string.
        /// </summary>
        /// <param name="defaultValue">specifies the default value to be used.</param>
        /// <returns>the extractor for further configuration and usage.</returns>
        public DslJsonExtractor DefaultValue(string defaultValue)
        {
            _defaultValue = defaultValue;
            return this;
        }

        /// <summary>
        /// Allows selecting the query language to use for extracting values from a given JSON.
        /// </summary>
        /// <param name="queryLanguage">specifies the query language to use to extracting values. When no value is
        /// specified, JMESPath is used by default.</param>
        /// <returns>the extractor for further configuration and usage.</returns>
        /// <seealso cref="DslJsonQueryLanguage"/>
        public DslJsonExtractor QueryLanguage(DslJsonQueryLanguage queryLanguage)
        {
            _queryLanguage = queryLanguage;
            return this;
        }
    }
}
