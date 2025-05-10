using RuleEngine.Core.Enums;

namespace RuleEngine.Core
{
    public class RuleExecutionResult
    {
        /// <summary>
        /// Gets or sets the total execution time.
        /// </summary>
        /// <value>
        /// The total execution time.
        /// </value>
        public TimeSpan TotalExecutionTime { get; set; }

        /// <summary>
        /// Gets the logs.
        /// </summary>
        /// <value>
        /// The logs.
        /// </value>
        public List<RuleExecutionLog> Logs { get; } = new List<RuleExecutionLog>();

        /// <summary>
        /// Adds the log.
        /// </summary>
        /// <param name="ruleName">Name of the rule.</param>
        /// <param name="result">The result.</param>
        /// <param name="durationMs">The duration ms.</param>
        public void AddLog(string ruleName, RuleResult result, long durationMs)
        {
            Logs.Add(new RuleExecutionLog(ruleName, result, durationMs));
        }

        /// <summary>
        /// Anies the failures.
        /// </summary>
        /// <returns></returns>
        public bool AnyFailures() => Logs.Any(l => l.Result == RuleResult.Fail);

        /// <summary>
        /// Determines whether this instance has errors.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance has errors; otherwise, <c>false</c>.
        /// </returns>
        public bool HasErrors() => Logs.Any(l => l.Result == RuleResult.Error);
    }
}
