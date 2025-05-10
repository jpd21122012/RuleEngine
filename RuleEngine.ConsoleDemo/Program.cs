// Load rules
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using RuleEngine.Core;
using RuleEngine.Core.Enums;

System.Globalization.CultureInfo.CurrentCulture = new System.Globalization.CultureInfo("en-US");
Console.OutputEncoding = System.Text.Encoding.UTF8;

using var loggerFactory = LoggerFactory.Create(builder =>
{
    builder.AddSimpleConsole(options =>
    {
        options.SingleLine = true;
        options.TimestampFormat = "HH:mm:ss ";
        options.ColorBehavior = LoggerColorBehavior.Enabled;
    });
    builder.SetMinimumLevel(LogLevel.Debug);
});


try
{
    var logger = loggerFactory.CreateLogger<Program>();
    logger.LogInformation("Starting rule engine demo...");

    // Initialize services
    var ruleLoader = new RuleLoader(loggerFactory.CreateLogger<RuleLoader>());
    var rules = ruleLoader.LoadRulesFromJson("Rules/sample-rules.json");
    var engine = new RulesEngine(rules);

    // Setup context
    var context = new RuleContext();
    context.Set("Age", 25);
    context.Set("OrderTotal", 250.00m);
    context.Set("IsPremiumMember", true);

    // Execute
    var result = engine.Execute(context);

    // Display results
    logger.LogInformation("Execution completed in {ElapsedMs:F2}ms",
        result.TotalExecutionTime.TotalMilliseconds);

    foreach (var log in result.Logs)
    {
        var status = log.Result == RuleResult.Success ? "✅" : "❌";
        logger.LogInformation("{Status} {LogEntry}", status, log.ToString());
    }

    // Show final context state with proper formatting
    logger.LogInformation("Final Context:");
    logger.LogInformation("- IsAdult: {IsAdult}", context.Get<bool>("IsAdult"));
    logger.LogInformation("- Discount: {Discount,10:$#,##0.00;($#,##0.00)}", context.Get<decimal>("Discount"));


    // After rule execution
    logger.LogInformation("\nExecution Summary:");
    logger.LogInformation("Total Rules: {0}", result.Logs.Count);
    logger.LogInformation("Successful: {0}", result.Logs.Count(l => l.Result == RuleResult.Success));
    logger.LogInformation("Total Duration: {0:0.##}ms", result.TotalExecutionTime.TotalMilliseconds);
}
catch (Exception ex)
{
    Console.WriteLine($"Fatal error: {ex}");
}

Console.ReadLine();