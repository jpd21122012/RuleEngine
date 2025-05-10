using FluentAssertions;
using RuleEngine.Core;
using RuleEngine.Core.Enums;

namespace RuleEngine.Tests.Core
{
    public class RuleContextTests
    {
        /// <summary>
        /// Sets the and get should store and retrieve values.
        /// </summary>
        [Fact]
        public void SetAndGet_ShouldStoreAndRetrieveValues()
        {
            // Arrange
            var context = new RuleContext();

            // Act
            context.Set("TestInt", 42);
            context.Set("TestString", "Hello");

            // Assert
            context.Get<int>("TestInt").Should().Be(42);
            context.Get<string>("TestString").Should().Be("Hello");
            context.Get<bool>("NonExistent").Should().BeFalse();
        }

        /// <summary>
        /// Logs the rule execution should maintain execution log.
        /// </summary>
        [Fact]
        public void LogRuleExecution_ShouldMaintainExecutionLog()
        {
            // Arrange
            var context = new RuleContext();

            // Act
            context.LogRuleExecution("Rule1", RuleResult.Success,1000, "Test message");

            // Assert
            var logs = context.GetExecutionLog();
            logs.Should().ContainSingle();
            logs.First().RuleName.Should().Be("Rule1");
        }
    }
}
