using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using RuleEngine.Core;

namespace RuleEngine.Tests.Integration
{
    public class FullWorkflowTests
    {
        /// <summary>
        /// Executes the should process rules correctly for valid input.
        /// </summary>
        [Fact]
        public void Execute_ShouldProcessRulesCorrectly_ForValidInput()
        {
            // Arrange
            var loggerFactory = Mock.Of<ILoggerFactory>();
            var loader = new RuleLoader(Mock.Of<ILogger<RuleLoader>>());
            var rules = loader.LoadRulesFromJson("TestData/workflow-rules.json");
            var engine = new RulesEngine(rules);

            var context = new RuleContext();
            context.Set("Age", 25);
            context.Set("OrderTotal", 150.00m);
            context.Set("IsPremiumMember", true);

            // Act
            var result = engine.Execute(context);

            // Assert
            result.Logs.Should().HaveCount(3);
            context.Get<bool>("IsAdult").Should().BeTrue();
            context.Get<decimal>("DiscountAmount").Should().Be(15.00m);
        }
    }
}
