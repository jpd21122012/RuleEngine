using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using RuleEngine.Core;

namespace RuleEngine.Tests.Services
{
    public class RuleLoaderTests
    {
        /// <summary>
        /// Loads the rules from json should return rules when valid file provided.
        /// </summary>
        [Fact]
        public void LoadRulesFromJson_ShouldReturnRules_WhenValidFileProvided()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<RuleLoader>>();
            var loader = new RuleLoader(loggerMock.Object);

            // Act
            var rules = loader.LoadRulesFromJson("TestData/valid-rules.json");

            // Assert
            rules.Should().NotBeEmpty();
            rules.Should().AllBeOfType<DynamicRule>();
        }

        /// <summary>
        /// Loads the rules from json should log error when invalid rule exists.
        /// </summary>
        [Fact]
        public void LoadRulesFromJson_ShouldLogError_WhenInvalidRuleExists()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<RuleLoader>>();
            var loader = new RuleLoader(loggerMock.Object);

            // Act
            var rules = loader.LoadRulesFromJson("TestData/invalid-rules.json");

            // Assert
            loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
                Times.AtLeastOnce);
        }
    }
}
