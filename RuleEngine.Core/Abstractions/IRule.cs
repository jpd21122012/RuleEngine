using RuleEngine.Core.Enums;

namespace RuleEngine.Core.Abstractions
{
    public interface IRule
    {
        string Name { get; }
        string Description { get; }
        int Priority { get; }
        bool IsEnabled { get; }

        bool ShouldEvaluate(RuleContext context);
        RuleResult Evaluate(RuleContext context);
        void Execute(RuleContext context);
    }
}
