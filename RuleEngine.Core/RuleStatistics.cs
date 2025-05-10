using RuleEngine.Core.Enums;
using System.Collections.Concurrent;

namespace RuleEngine.Core
{
    public class RuleStatistics
    {
        /// <summary>
        /// The stats
        /// </summary>
        private readonly ConcurrentDictionary<string, RuleStats> _stats = new();

        /// <summary>
        /// Records the execution.
        /// </summary>
        /// <param name="ruleName">Name of the rule.</param>
        /// <param name="result">The result.</param>
        /// <param name="duration">The duration.</param>
        public void RecordExecution(string ruleName, RuleResult result, TimeSpan duration)
        {
            var stats = _stats.GetOrAdd(ruleName, _ => new RuleStats());
            stats.RecordExecution(result, duration);
        }

        /// <summary>
        /// Gets the rule stats.
        /// </summary>
        /// <param name="ruleName">Name of the rule.</param>
        /// <returns></returns>
        public RuleStats GetRuleStats(string ruleName)
        {
            return _stats.TryGetValue(ruleName, out var stats) ? stats : new RuleStats();
        }

        //
        public class RuleStats
        {
            private int _totalExecutions;
            private int _successCount;
            private int _failCount;
            private int _errorCount;
            private long _totalDurationTicks;

            public double SuccessRate => _totalExecutions > 0 ? (double)_successCount / _totalExecutions : 0;
            public double FailRate => _totalExecutions > 0 ? (double)_failCount / _totalExecutions : 0;
            public double ErrorRate => _totalExecutions > 0 ? (double)_errorCount / _totalExecutions : 0;

            /// <summary>
            /// Gets the average execution time.
            /// </summary>
            /// <value>
            /// The average execution time.
            /// </value>
            public TimeSpan AverageExecutionTime => _totalExecutions > 0
                ? TimeSpan.FromTicks(_totalDurationTicks / _totalExecutions)
                : TimeSpan.Zero;

            /// <summary>
            /// Records the execution.
            /// </summary>
            /// <param name="result">The result.</param>
            /// <param name="duration">The duration.</param>
            public void RecordExecution(RuleResult result, TimeSpan duration)
            {
                Interlocked.Increment(ref _totalExecutions);
                Interlocked.Add(ref _totalDurationTicks, duration.Ticks);

                switch (result)
                {
                    case RuleResult.Success:
                        Interlocked.Increment(ref _successCount);
                        break;
                    case RuleResult.Fail:
                        Interlocked.Increment(ref _failCount);
                        break;
                    case RuleResult.Error:
                        Interlocked.Increment(ref _errorCount);
                        break;
                }
            }
        }
    }
}
