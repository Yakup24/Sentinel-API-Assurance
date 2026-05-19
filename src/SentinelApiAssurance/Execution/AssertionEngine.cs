using System.Xml.Linq;
using SentinelApiAssurance.Models;
using SentinelApiAssurance.Services;

namespace SentinelApiAssurance.Execution;

public static class AssertionEngine
{
    public static List<AssertionResult> Evaluate(TestCase testCase, RawTestResponse response)
    {
        var results = new List<AssertionResult>();

        foreach (var assertion in testCase.Assertions)
        {
            results.Add(EvaluateSingle(assertion, response));
        }

        return results;
    }

    private static AssertionResult EvaluateSingle(AssertionDefinition assertion, RawTestResponse response)
    {
        return assertion.Type.ToLowerInvariant() switch
        {
            "contains" => Contains(assertion, response),
            "notcontains" => NotContains(assertion, response),
            "nosoapfault" => NoSoapFault(response),
            "xmlelementexists" => XmlElementExists(assertion, response),
            "xmlelementequals" => XmlElementEquals(assertion, response),
            "maxdurationms" => MaxDuration(assertion, response),
            _ => new AssertionResult
            {
                Type = assertion.Type,
                Passed = false,
                Message = $"Unknown assertion type: {assertion.Type}"
            }
        };
    }

    private static AssertionResult Contains(AssertionDefinition assertion, RawTestResponse response)
    {
        var expected = assertion.Value ?? "";
        var passed = response.Body.Contains(expected, StringComparison.OrdinalIgnoreCase);

        return new AssertionResult
        {
            Type = "Contains",
            Passed = passed,
            Message = passed ? $"Response contains expected value: {expected}" : $"Response does not contain expected value: {expected}"
        };
    }

    private static AssertionResult NotContains(AssertionDefinition assertion, RawTestResponse response)
    {
        var unexpected = assertion.Value ?? "";
        var passed = !response.Body.Contains(unexpected, StringComparison.OrdinalIgnoreCase);

        return new AssertionResult
        {
            Type = "NotContains",
            Passed = passed,
            Message = passed ? $"Response does not contain blocked value: {unexpected}" : $"Response contains blocked value: {unexpected}"
        };
    }

    private static AssertionResult NoSoapFault(RawTestResponse response)
    {
        var hasFault =
            response.Body.Contains(":Fault", StringComparison.OrdinalIgnoreCase) ||
            response.Body.Contains("<Fault", StringComparison.OrdinalIgnoreCase) ||
            response.Body.Contains("faultstring", StringComparison.OrdinalIgnoreCase);

        return new AssertionResult
        {
            Type = "NoSoapFault",
            Passed = !hasFault,
            Message = hasFault ? "SOAP Fault detected." : "SOAP Fault not detected."
        };
    }

    private static AssertionResult XmlElementExists(AssertionDefinition assertion, RawTestResponse response)
    {
        var elementName = assertion.ElementName ?? "";
        var exists = TryReadXml(response.Body, out var doc) &&
                     doc!.Descendants().Any(x => x.Name.LocalName.Equals(elementName, StringComparison.OrdinalIgnoreCase));

        return new AssertionResult
        {
            Type = "XmlElementExists",
            Passed = exists,
            Message = exists ? $"XML element exists: {elementName}" : $"XML element not found: {elementName}"
        };
    }

    private static AssertionResult XmlElementEquals(AssertionDefinition assertion, RawTestResponse response)
    {
        var elementName = assertion.ElementName ?? "";
        var expected = assertion.Expected ?? "";

        var passed = false;
        if (TryReadXml(response.Body, out var doc))
        {
            var value = doc!.Descendants()
                .FirstOrDefault(x => x.Name.LocalName.Equals(elementName, StringComparison.OrdinalIgnoreCase))
                ?.Value
                ?.Trim();

            passed = string.Equals(value, expected, StringComparison.OrdinalIgnoreCase);
        }

        return new AssertionResult
        {
            Type = "XmlElementEquals",
            Passed = passed,
            Message = passed ? $"XML element has expected value: {elementName}={expected}" : $"XML element value mismatch: {elementName}={expected}"
        };
    }

    private static AssertionResult MaxDuration(AssertionDefinition assertion, RawTestResponse response)
    {
        var max = assertion.MaxDurationMs ?? 0;
        var passed = max <= 0 || response.DurationMs <= max;

        return new AssertionResult
        {
            Type = "MaxDurationMs",
            Passed = passed,
            Message = passed ? $"Duration is within threshold: {response.DurationMs} ms <= {max} ms" : $"Duration exceeded threshold: {response.DurationMs} ms > {max} ms"
        };
    }

    private static bool TryReadXml(string body, out XDocument? document)
    {
        try
        {
            document = XDocument.Parse(body);
            return true;
        }
        catch
        {
            document = null;
            return false;
        }
    }
}
