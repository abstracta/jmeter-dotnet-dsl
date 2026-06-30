using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using FluentAssertions;
using FluentAssertions.Execution;
using WireMock;
using WireMock.FluentAssertions;
using WireMock.Types;

namespace Abstracta.JmeterDsl
{
    public static class WireMockAssertionsExtensions
    {
        public static AndConstraint<WireMockAssertions> WithBody(this WireMockAssertions instance, string body) =>
            WithBody(instance, request => string.Equals(request.Body, body, StringComparison.OrdinalIgnoreCase), body);

        private static AndConstraint<WireMockAssertions> WithBody(WireMockAssertions instance, Func<IRequestMessage, bool> predicate, object body)
        {
            var (filter, condition) = instance.BuildFilterAndCondition(predicate);

            Execute.Assertion
                .BecauseOf(string.Empty, Array.Empty<object>())
                .Given(() => instance.RequestMessages)
                .ForCondition(requests => instance.CallsCount == 0 || requests.Any())
                .FailWith(
                    "Expected {context:wiremockserver} to have been called using body " + (body is Regex ? "matching " : string.Empty) + "{0}{reason}, but no calls were made.",
                    body
                )
                .Then
                .ForCondition(condition)
                .FailWith(
                    "Expected {context:wiremockserver} to have been called using body " + (body is Regex ? "matching " : string.Empty) + "{0}{reason}, but didn't find it among the bodies {1}.",
                    _ => body,
                    requests => requests.Select(request => request.Body)
                );
            instance.FilterRequestMessages(filter);
            return new AndConstraint<WireMockAssertions>(instance);
        }

        public static AndConstraint<WireMockAssertions> WithoutHeader(this WireMockAssertions instance, string headerName) =>
            instance.WithoutHeaderKey(headerName);

        public static AndConstraint<WireMockAssertions> WithHeader(this WireMockAssertions instance, string headerName, Regex valueRegex)
        {
            var (filter, condition) = instance.BuildFilterAndCondition(request =>
            {
                var headers = request.Headers?.ToArray() ?? Array.Empty<KeyValuePair<string, WireMockList<string>>>();
                var matchingHeaderValues = headers
                    .Where(h => h.Key == headerName)
                    .SelectMany(h => h.Value.ToArray())
                    .ToArray();
                return matchingHeaderValues.Length == 1 && valueRegex.IsMatch(matchingHeaderValues[0]);
            });

            Execute.Assertion
                .BecauseOf(string.Empty, Array.Empty<object>())
                .Given(() => instance.RequestMessages)
                .ForCondition(requests => instance.CallsCount == 0 || requests.Any())
                .FailWith(
                    "Expected {context:wiremockserver} to have been called with Header {0} matching {1}{reason}.",
                    headerName,
                    valueRegex
                )
                .Then
                .ForCondition(condition)
                .FailWith(
                    "Expected {context:wiremockserver} to have been called with Header {0} matching {1}{reason}, but didn't find it among the calls with Header(s) {2}.",
                    _ => headerName,
                    _ => valueRegex,
                    requests => requests.Select(request => request.Headers)
                );
            instance.FilterRequestMessages(filter);
            return new AndConstraint<WireMockAssertions>(instance);
        }

        public static AndConstraint<WireMockAssertions> WithBody(this WireMockAssertions instance, Regex bodyRegex) =>
            WithBody(instance, request => bodyRegex.IsMatch(request.Body), bodyRegex);
    }
}
