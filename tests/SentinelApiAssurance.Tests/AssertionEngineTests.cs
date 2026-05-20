using SentinelApiAssurance.Execution;
using SentinelApiAssurance.Models;
using SentinelApiAssurance.Services;

namespace SentinelApiAssurance.Tests;

public sealed class AssertionEngineTests
{
    [Fact]
    public void Evaluate_Passes_HttpStatus_Contains_And_MaxDuration()
    {
        var testCase = new TestCase
        {
            Assertions =
            {
                new AssertionDefinition { Type = "HttpStatus", Expected = "200" },
                new AssertionDefinition { Type = "Contains", Value = "accepted" },
                new AssertionDefinition { Type = "MaxDurationMs", MaxDurationMs = 100 }
            }
        };

        var response = new RawTestResponse
        {
            HttpStatus = 200,
            Body = "{ \"result\": \"accepted\" }",
            DurationMs = 42
        };

        var results = AssertionEngine.Evaluate(testCase, response);

        Assert.All(results, result => Assert.True(result.Passed, result.Message));
    }

    [Fact]
    public void Evaluate_Detects_Blocked_Text_With_NotContains()
    {
        var testCase = new TestCase
        {
            Assertions =
            {
                new AssertionDefinition { Type = "NotContains", Value = "Exception" }
            }
        };

        var response = new RawTestResponse { Body = "System.Exception: demo failure" };

        var result = AssertionEngine.Evaluate(testCase, response).Single();

        Assert.False(result.Passed);
    }

    [Fact]
    public void Evaluate_Supports_Xml_Element_Assertions()
    {
        var testCase = new TestCase
        {
            Assertions =
            {
                new AssertionDefinition { Type = "XmlElementExists", ElementName = "customerId" },
                new AssertionDefinition { Type = "XmlElementEquals", ElementName = "status", Expected = "ACTIVE" }
            }
        };

        var response = new RawTestResponse
        {
            Body = "<response><customerId>CUST-001</customerId><status>ACTIVE</status></response>"
        };

        var results = AssertionEngine.Evaluate(testCase, response);

        Assert.All(results, result => Assert.True(result.Passed, result.Message));
    }

    [Fact]
    public void Evaluate_Supports_Json_Field_Assertions()
    {
        var testCase = new TestCase
        {
            Assertions =
            {
                new AssertionDefinition { Type = "JsonFieldExists", ElementName = "customer.id" },
                new AssertionDefinition { Type = "JsonFieldEquals", ElementName = "customer.status", Expected = "active" }
            }
        };

        var response = new RawTestResponse
        {
            Body = """{"customer":{"id":"CUST-001","status":"active"}}"""
        };

        var results = AssertionEngine.Evaluate(testCase, response);

        Assert.All(results, result => Assert.True(result.Passed, result.Message));
    }
}
