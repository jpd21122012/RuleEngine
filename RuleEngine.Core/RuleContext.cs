using RuleEngine.Core.Enums;
using System.Collections.Concurrent;

namespace RuleEngine.Core
{
    public class RuleContext
    {
        /// <summary>
        /// The data
        /// </summary>
        private readonly ConcurrentDictionary<string, object> _data = new();

        /// <summary>
        /// The execution log
        /// </summary>
        private readonly ConcurrentStack<RuleExecutionLog> _executionLog = new();

        /// <summary>
        /// Gets the evaluation time.
        /// </summary>
        /// <value>
        /// The evaluation time.
        /// </value>
        public DateTime EvaluationTime { get; } = DateTime.UtcNow;

        /// <summary>
        /// Gets the session identifier.
        /// </summary>
        /// <value>
        /// The session identifier.
        /// </value>
        public Guid SessionId { get; } = Guid.NewGuid();

        /// <summary>
        /// Gets or sets the <see cref="System.Object"/> with the specified key.
        /// </summary>
        /// <value>
        /// The <see cref="System.Object"/>.
        /// </value>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public object this[string key]
        {
            get => _data.TryGetValue(key, out var value) ? value : null;
            set => _data[key] = value;
        }

        // Parser-friendly methods
        public int GetInt(string key) => Get<int>(key);
        public decimal GetDecimal(string key) => Get<decimal>(key);
        public bool GetBool(string key) => Get<bool>(key);
        public string GetString(string key) => Get<string>(key);

        /// <summary>
        /// Gets the specified key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public T Get<T>(string key) =>
            _data.TryGetValue(key, out var value) ? (T)value : default;

        /// <summary>
        /// Sets the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void Set(string key, object value) => _data[key] = value;

        /// <summary>
        /// Logs the rule execution.
        /// </summary>
        /// <param name="ruleName">Name of the rule.</param>
        /// <param name="result">The result.</param>
        /// <param name="durationMs">The duration ms.</param>
        /// <param name="message">The message.</param>
        public void LogRuleExecution(string ruleName, RuleResult result, long durationMs, string message = "")
        {
            _executionLog.Push(new RuleExecutionLog(
    ruleName,
    result,
    durationMs,
    message));
        }

        /// <summary>
        /// Gets the execution log.
        /// </summary>
        /// <returns></returns>
        public IReadOnlyCollection<RuleExecutionLog> GetExecutionLog() => _executionLog.ToList().AsReadOnly();
    }

    public record RuleExecutionLog(string RuleName, RuleResult Result, long DurationMs, string Message = "")
    {
        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var messagePart = string.IsNullOrEmpty(Message) ? "" : $" - {Message}";
            return $"{RuleName}: {Result} ({FormatDuration(DurationMs)}){messagePart}";
        }

        /// <summary>
        /// Formats the duration.
        /// </summary>
        /// <param name="ms">The ms.</param>
        /// <returns></returns>
        private static string FormatDuration(long ms) => ms < 1 ? "<1ms" : $"{ms}ms";
    }
}
