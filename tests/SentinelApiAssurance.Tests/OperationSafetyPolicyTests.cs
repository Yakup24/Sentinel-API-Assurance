using SentinelApiAssurance.Models;
using SentinelApiAssurance.Safety;

namespace SentinelApiAssurance.Tests;

public sealed class OperationSafetyPolicyTests
{
    [Theory]
    [InlineData("deleteCustomer")]
    [InlineData("updateCustomer")]
    [InlineData("deactivateSubscription")]
    [InlineData("submitOrder")]
    [InlineData("paymentCapture")]
    public void ShouldBlock_Blocks_StateChanging_Operations_By_Default(string operation)
    {
        var policy = new OperationSafetyPolicy(new AppConfig());
        var testCase = new TestCase { Operation = operation };

        var blocked = policy.ShouldBlock(testCase, out var reason);

        Assert.True(blocked);
        Assert.Contains("State-changing", reason);
    }

    [Theory]
    [InlineData("getCustomer")]
    [InlineData("readInvoice")]
    [InlineData("searchProducts")]
    [InlineData("isServiceSubscriber")]
    public void ShouldBlock_Allows_ReadOnly_Prefixes(string operation)
    {
        var policy = new OperationSafetyPolicy(new AppConfig());
        var testCase = new TestCase { Operation = operation };

        var blocked = policy.ShouldBlock(testCase, out _);

        Assert.False(blocked);
    }

    [Fact]
    public void ShouldBlock_Allows_Explicitly_Approved_StateChanging_Operation()
    {
        var policy = new OperationSafetyPolicy(new AppConfig());
        var testCase = new TestCase
        {
            Operation = "updateCustomer",
            AllowStateChangingOperation = true
        };

        var blocked = policy.ShouldBlock(testCase, out _);

        Assert.False(blocked);
    }
}
