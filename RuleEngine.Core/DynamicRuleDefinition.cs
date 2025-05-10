namespace RuleEngine.Core
{
    public class DynamicRuleDefinition
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Priority { get; set; } = 100;
        public bool IsEnabled { get; set; } = true;
        public string Condition { get; set; }
        public string Action { get; set; }
    }
}
