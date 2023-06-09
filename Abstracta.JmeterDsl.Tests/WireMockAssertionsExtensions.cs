using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using FluentAssertions.Execution;
using WireMock;
using WireMock.FluentAssertions;
using WireMock.Types;

namespace Abstracta.JmeterDsl
{
    public static class WireMockAssertionsExtensions
    {
        public static AndConstraint<WireMockAssertions> WithBody(this WireMockAssertions instance, string body)
        {
            var requestsField = GetPrivateField("_requestMessages", instance);
            var requests = (IReadOnlyList<IRequestMessage>)requestsField.GetValue(instance)!;
            var callsCount = (int?)GetPrivateField("_callsCount", instance).GetValue(instance);
            Func<IRequestMessage, bool> predicate = request => string.Equals(request.Body, body, StringComparison.OrdinalIgnoreCase);
            Func<IReadOnlyList<IRequestMessage>, IReadOnlyList<IRequestMessage>> filter = requests => requests.Where(predicate).ToList();
            Func<IReadOnlyList<IRequestMessage>, bool> condition = requests => (callsCount is null && filter(requests).Any()) || callsCount == filter(requests).Count;

            Execute.Assertion
                .BecauseOf(string.Empty, Array.Empty<object>())
                .Given(() => requests)
                .ForCondition(requests => callsCount == 0 || requests.Any())
                .FailWith(
                    "Expected {context:wiremockserver} to have been called using body {0}{reason}, but no calls were made.",
                    body
                )
                .Then
                .ForCondition(condition)
                .FailWith(
                    "Expected {context:wiremockserver} to have been called using body {0}{reason}, but didn't find it among the bodies {1}.",
                    _ => body,
                    requests => requests.Select(request => request.Body)
                );
            requestsField.SetValue(instance, filter(requests).ToList());
            return new AndConstraint<WireMockAssertions>(instance);
        }

        private static FieldInfo GetPrivateField(string fieldName, object o) =>
            o.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance)!;

        public static AndConstraint<WireMockAssertions> WithoutHeader(this WireMockAssertions instance, string headerName)
        {
            var headersField = GetPrivateField("_headers", instance);
            var headers = (IReadOnlyList<KeyValuePair<string, WireMockList<string>>>)headersField.GetValue(instance)!;
            using (new AssertionScope("headers from requests sent"))
            {
                headers.Select(h => h.Key).Should().NotContain(headerName);
            }
            return new AndConstraint<WireMockAssertions>(instance);
        }
    }
}
