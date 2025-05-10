using RuleEngine.Core.Abstractions;
using RuleEngine.Core.Enums;
using System.Diagnostics;
using System.Linq.Dynamic.Core;
using System.Linq.Dynamic.Core.CustomTypeProviders;
using System.Linq.Expressions;

namespace RuleEngine.Core
{

    public class DynamicRule : IRule
    {
        /// <summary>
        /// The compiled condition
        /// </summary>
        private Func<RuleContext, bool> _compiledCondition;
        /// <summary>
        /// The compiled action
        /// </summary>
        private Action<RuleContext> _compiledAction;

        public string Name { get; set; }
        public string Description { get; set; }
        public int Priority { get; set; } = 100;
        public bool IsEnabled { get; set; } = true;

        public string ConditionExpression { get; set; }
        public string ActionExpression { get; set; }

        /// <summary>
        /// Shoulds the evaluate.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public bool ShouldEvaluate(RuleContext context) => IsEnabled;

        /// <summary>
        /// Compiles this instance.
        /// </summary>
        /// <exception cref="RuleEngine.Core.DynamicRule.RuleCompilationException">Failed to compile rule {Name}</exception>
        public void Compile()
        {
            var config = new ParsingConfig
            {
                CustomTypeProvider = new CustomTypeProvider(),
                IsCaseSensitive = false
            };

            var ctxParam = Expression.Parameter(typeof(RuleContext), "ctx");

            try
            {
                // Compile condition
                var conditionExpr = DynamicExpressionParser.ParseLambda(
                    config,
                    new[] { ctxParam },
                    typeof(bool),
                    ConditionExpression);
                _compiledCondition = (Func<RuleContext, bool>)conditionExpr.Compile();

                // Compile action if exists
                if (!string.IsNullOrWhiteSpace(ActionExpression))
                {
                    var actionExpr = DynamicExpressionParser.ParseLambda(
                        config,
                        new[] { ctxParam },
                        typeof(void),
                        ActionExpression);
                    _compiledAction = (Action<RuleContext>)actionExpr.Compile();
                }
            }
            catch (Exception ex)
            {
                throw new RuleCompilationException($"Failed to compile rule {Name}", ex);
            }
        }

        //
        private class CustomTypeProvider : DefaultDynamicLinqCustomTypeProvider
        {
            /// <summary>
            /// </summary>
            /// <returns></returns>
            /// <inheritdoc cref="M:System.Linq.Dynamic.Core.CustomTypeProviders.IDynamicLinqCustomTypeProvider.GetCustomTypes" />
            public override HashSet<Type> GetCustomTypes() =>
                new HashSet<Type>
                {
                typeof(RuleContext),
                typeof(Math), // Allows using Math functions in rules
                typeof(Decimal)
                };
        }

        //
        public class RuleCompilationException : Exception
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="RuleCompilationException"/> class.
            /// </summary>
            /// <param name="message">The message.</param>
            /// <param name="inner">The inner.</param>
            public RuleCompilationException(string message, Exception inner)
                : base(message, inner) { }
        }

        /// <summary>
        /// Evaluates the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public RuleResult Evaluate(RuleContext context)
        {
            try
            {
                return _compiledCondition(context)
                    ? RuleResult.Success
                    : RuleResult.Fail;
            }
            catch
            {
                return RuleResult.Error;
            }
        }

        /// <summary>
        /// Executes the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        public void Execute(RuleContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                _compiledAction?.Invoke(context);
                stopwatch.Stop();

                context.LogRuleExecution(
                    Name,
                    RuleResult.Success,
                    stopwatch.ElapsedMilliseconds,
                    $"Action: {ActionExpression.Truncate(50)}"); // Truncate long expressions
            }
            catch
            {
                stopwatch.Stop();
                context.LogRuleExecution(
                    Name,
                    RuleResult.Error,
                    stopwatch.ElapsedMilliseconds);
                throw;
            }
        }
    }

    //
    public static class StringExtensions
    {
        /// <summary>
        /// Truncates the specified maximum length.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="maxLength">The maximum length.</param>
        /// <returns></returns>
        public static string Truncate(this string value, int maxLength) =>
            string.IsNullOrEmpty(value) ? value :
            value.Length <= maxLength ? value :
            value[..(maxLength - 3)] + "...";
    }
}
