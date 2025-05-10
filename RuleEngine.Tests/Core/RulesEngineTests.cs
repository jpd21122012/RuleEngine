using Moq;
using RuleEngine.Core.Abstractions;
using RuleEngine.Core.Enums;
using RuleEngine.Core;
using FluentAssertions;

namespace RuleEngine.Tests.Core
{
    public class RulesEngineTests
    {
        /// <summary>
        /// The mock rule1
        /// </summary>
        private readonly Mock<IRule> _mockRule1;
        /// <summary>
        /// The mock rule2
        /// </summary>
        private readonly Mock<IRule> _mockRule2;
        /// <summary>
        /// The engine
        /// </summary>
        private readonly RulesEngine _engine;

        /// <summary>
        /// Initializes a new instance of the <see cref="RulesEngineTests"/> class.
        /// </summary>
        public RulesEngineTests()
        {
            // Setup mock rules
            _mockRule1 = new Mock<IRule>();
            _mockRule1.Setup(r => r.Name).Returns("Rule1");
            _mockRule1.Setup(r => r.Priority).Returns(1);
            _mockRule1.Setup(r => r.IsEnabled).Returns(true);
            _mockRule1.Setup(r => r.ShouldEvaluate(It.IsAny<RuleContext>())).Returns(true);

            _mockRule2 = new Mock<IRule>();
            _mockRule2.Setup(r => r.Name).Returns("Rule2");
            _mockRule2.Setup(r => r.Priority).Returns(2);
            _mockRule2.Setup(r => r.IsEnabled).Returns(true);
            _mockRule2.Setup(r => r.ShouldEvaluate(It.IsAny<RuleContext>())).Returns(true);

            // Create engine with mock rules
            _engine = new RulesEngine(new[] { _mockRule1.Object, _mockRule2.Object });
        }

        /// <summary>
        /// Executes the should evaluate rules in priority order.
        /// </summary>
        [Fact]
        public void Execute_ShouldEvaluateRulesInPriorityOrder()
        {
            // Arrange
            var context = new RuleContext();
            _mockRule1.Setup(r => r.Evaluate(context)).Returns(RuleResult.Success);
            _mockRule2.Setup(r => r.Evaluate(context)).Returns(RuleResult.Success);

            // Act
            var result = _engine.Execute(context);

            // Assert
            result.Logs.Select(l => l.RuleName).Should().ContainInOrder("Rule1", "Rule2");
        }

        /// <summary>
        /// Executes the should skip disabled rules.
        /// </summary>
        [Fact]
        public void Execute_ShouldSkipDisabledRules()
        {
            // Arrange
            var context = new RuleContext();
            _mockRule1.Setup(r => r.IsEnabled).Returns(false);

            // Act
            var result = _engine.Execute(context);

            // Assert
            result.Logs.Should().ContainSingle();
            result.Logs[0].RuleName.Should().Be("Rule2");
        }

        /// <summary>
        /// Executes the should skip rules when should evaluate returns false.
        /// </summary>
        [Fact]
        public void Execute_ShouldSkipRulesWhenShouldEvaluateReturnsFalse()
        {
            // Arrange
            var context = new RuleContext();
            _mockRule1.Setup(r => r.ShouldEvaluate(context)).Returns(false);

            // Act
            var result = _engine.Execute(context);

            // Assert
            result.Logs.Should().ContainSingle();
            result.Logs[0].Result.Should().Be(RuleResult.NotApplicable);
        }

        /// <summary>
        /// Executes the should execute action when evaluation succeeds.
        /// </summary>
        [Fact]
        public void Execute_ShouldExecuteAction_WhenEvaluationSucceeds()
        {
            // Arrange
            var context = new RuleContext();
            _mockRule1.Setup(r => r.Evaluate(context)).Returns(RuleResult.Success);
            _mockRule1.Setup(r => r.Execute(context)).Verifiable();

            // Act
            _engine.Execute(context);

            // Assert
            _mockRule1.Verify(r => r.Execute(context), Times.Once);
        }

        /// <summary>
        /// Executes the should not execute action when evaluation fails.
        /// </summary>
        [Fact]
        public void Execute_ShouldNotExecuteAction_WhenEvaluationFails()
        {
            // Arrange
            var context = new RuleContext();
            _mockRule1.Setup(r => r.Evaluate(context)).Returns(RuleResult.Fail);

            // Act
            _engine.Execute(context);

            // Assert
            _mockRule1.Verify(r => r.Execute(context), Times.Never);
        }

        /// <summary>
        /// Gets the statistics should track rule executions.
        /// </summary>
        [Fact]
        public void GetStatistics_ShouldTrackRuleExecutions()
        {
            // Arrange
            var context = new RuleContext();
            _mockRule1.Setup(r => r.Evaluate(context)).Returns(RuleResult.Success);
            _mockRule2.Setup(r => r.Evaluate(context)).Returns(RuleResult.Fail);

            // Act - Execute twice to accumulate stats
            _engine.Execute(context);
            _engine.Execute(context);
            var stats = _engine.GetStatistics();

            // Assert
            stats.GetRuleStats("Rule1").SuccessRate.Should().Be(1.0);
            stats.GetRuleStats("Rule2").FailRate.Should().Be(1.0);
        }

        /// <summary>
        /// Optimizes the rule order should reorder rules based on statistics.
        /// </summary>
        [Fact]
        public void OptimizeRuleOrder_ShouldReorderRulesBasedOnStatistics()
        {
            // Arrange
            var context = new RuleContext();

            // Rule1: Fast but low success rate
            _mockRule1.Setup(r => r.Evaluate(context)).Returns(RuleResult.Fail);
            _mockRule1.Setup(r => r.Priority).Returns(1);

            // Rule2: Slower but high success rate
            _mockRule2.Setup(r => r.Evaluate(context)).Returns(RuleResult.Success);
            _mockRule2.Setup(r => r.Priority).Returns(2);

            // Initial execution to gather stats
            _engine.Execute(context);

            // Act
            _engine.OptimizeRuleOrder();
            var result = _engine.Execute(context);

            // Assert - Rule2 should now execute first
            result.Logs.Select(l => l.RuleName).Should().ContainInOrder("Rule2", "Rule1");
        }

        /// <summary>
        /// Executes the should handle rule errors gracefully.
        /// </summary>
        [Fact]
        public void Execute_ShouldHandleRuleErrorsGracefully()
        {
            // Arrange
            var context = new RuleContext();
            _mockRule1.Setup(r => r.Evaluate(context)).Throws<Exception>();

            // Act
            var result = _engine.Execute(context);

            // Assert
            result.Logs.Should().Contain(l => l.Result == RuleResult.Error);
        }
    }
}
