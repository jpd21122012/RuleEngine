namespace RuleEngine.Core.Abstractions
{
    public interface IRuleEngine
    {
        RuleExecutionResult Execute(RuleContext context);
        RuleStatistics GetStatistics();
        void OptimizeRuleOrder();
    }
}
