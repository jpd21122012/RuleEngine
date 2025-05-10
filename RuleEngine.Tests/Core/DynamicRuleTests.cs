using RuleEngine.Core.Enums;
using RuleEngine.Core;
using FluentAssertions;

namespace RuleEngine.Tests.Core
{
    public class DynamicRuleTests
    {
        /// <summary>
        /// Evaluates the should return success when condition met.
        /// </summary>
        [Fact]
        public void Evaluate_ShouldReturnSuccess_WhenConditionMet()
        {
            // Arrange
            var rule = new DynamicRule
            {
                ConditionExpression = "ctx.Get<int>(\"Age\") >= 21",
                ActionExpression = "ctx.Set(\"CanDrink\", true)"
            };
            rule.Compile();

            var context = new RuleContext();
            context.Set("Age", 25);

            // Act
            var result = rule.Evaluate(context);

            // Assert
            result.Should().Be(RuleResult.Success);
        }

        /// <summary>
        /// Executes the should modify context when action provided.
        /// </summary>
        [Fact]
        public void Execute_ShouldModifyContext_WhenActionProvided()
        {
            // Arrange
            var rule = new DynamicRule
            {
                ConditionExpression = "true",
                ActionExpression = "ctx.Set(\"TestValue\", 100)"
            };
            rule.Compile();

            var context = new RuleContext();

            // Act
            rule.Execute(context);

            // Assert
            context.Get<int>("TestValue").Should().Be(100);
        }
    }
}
