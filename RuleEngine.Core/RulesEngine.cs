using RuleEngine.Core.Abstractions;
using RuleEngine.Core.Enums;
using System.Diagnostics;

namespace RuleEngine.Core
{
    public class RulesEngine
    {
        /// <summary>
        /// The rules
        /// </summary>
        private readonly List<IRule> _rules;

        /// <summary>
        /// The statistics
        /// </summary>
        private readonly RuleStatistics _statistics = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="RulesEngine"/> class.
        /// </summary>
        /// <param name="rules">The rules.</param>
        public RulesEngine(IEnumerable<IRule> rules)
        {
            _rules = rules.OrderBy(r => r.Priority).ToList();
        }

        /// <summary>
        /// Executes the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public RuleExecutionResult Execute(RuleContext context)
        {
            var result = new RuleExecutionResult();
            var stopwatch = Stopwatch.StartNew();

            foreach (var rule in _rules.Where(r => r.IsEnabled))
            {
                var ruleStopwatch = Stopwatch.StartNew();
                try
                {
                    if (!rule.ShouldEvaluate(context))
                    {
                        result.AddLog(rule.Name, RuleResult.NotApplicable, 0);
                        continue;
                    }

                    var evaluationResult = rule.Evaluate(context);

                    if (evaluationResult == RuleResult.Success)
                    {
                        rule.Execute(context);
                    }

                    result.AddLog(rule.Name, evaluationResult, ruleStopwatch.ElapsedMilliseconds);
                    _statistics.RecordExecution(rule.Name, evaluationResult, ruleStopwatch.Elapsed);
                }
                finally
                {
                    ruleStopwatch.Stop();
                }
            }

            stopwatch.Stop();
            result.TotalExecutionTime = stopwatch.Elapsed;
            return result;
        }

        /// <summary>
        /// Optimizes the rule order.
        /// </summary>
        public void OptimizeRuleOrder()
        {
            // Reorder rules based on execution statistics
            _rules.Sort((a, b) =>
            {
                var aStats = _statistics.GetRuleStats(a.Name);
                var bStats = _statistics.GetRuleStats(b.Name);

                // Higher success rate rules first
                if (Math.Abs(aStats.SuccessRate - bStats.SuccessRate) > 0.01)
                    return bStats.SuccessRate.CompareTo(aStats.SuccessRate);

                // Then by average execution time (faster first)
                return aStats.AverageExecutionTime.CompareTo(bStats.AverageExecutionTime);
            });
        }

        /// <summary>
        /// Gets the statistics.
        /// </summary>
        /// <returns></returns>
        public RuleStatistics GetStatistics() => _statistics;
    }
}
