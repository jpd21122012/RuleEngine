using Microsoft.Extensions.Logging;
using RuleEngine.Core.Abstractions;
using System.Text.Json;
using static RuleEngine.Core.DynamicRule;

namespace RuleEngine.Core
{
    public class RuleLoader
    {
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<RuleLoader> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="RuleLoader"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public RuleLoader(ILogger<RuleLoader> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Loads the rules from json.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns></returns>
        public List<IRule> LoadRulesFromJson(string filePath)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    AllowTrailingCommas = true,
                    ReadCommentHandling = JsonCommentHandling.Skip
                };

                var json = File.ReadAllText(filePath);
                var ruleDefinitions = JsonSerializer.Deserialize<List<DynamicRuleDefinition>>(json, options);

                var rules = new List<IRule>();
                foreach (var definition in ruleDefinitions)
                {
                    try
                    {
                        var rule = new DynamicRule
                        {
                            Name = definition.Name,
                            Description = definition.Description,
                            Priority = definition.Priority,
                            IsEnabled = definition.IsEnabled,
                            ConditionExpression = definition.Condition,
                            ActionExpression = definition.Action
                        };

                        rule.Compile(); // Now throws RuleCompilationException
                        rules.Add(rule);
                    }
                    catch (RuleCompilationException ex)
                    {
                        _logger.LogError(ex, $"Failed to compile rule {definition.Name}");
                        // Continue with other rules
                    }
                }

                return rules;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Failed to load rules");
                throw;
            }
        }
    }
}
