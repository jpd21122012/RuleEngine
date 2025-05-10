using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RuleEngine.Core;
using RuleEngine.Core.Abstractions;

namespace RuleEngine.API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class RulesController : ControllerBase
    {
        /// <summary>
        /// The engine
        /// </summary>
        private readonly IRuleEngine _engine;

        /// <summary>
        /// Initializes a new instance of the <see cref="RulesController"/> class.
        /// </summary>
        /// <param name="engine">The engine.</param>
        public RulesController(IRuleEngine engine)
        {
            _engine = engine;
        }

        /// <summary>
        /// Executes the rules.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        [HttpPost("execute")]
        public IActionResult ExecuteRules([FromBody] Dictionary<string, object> input)
        {
            var context = new RuleContext();
            foreach (var item in input)
            {
                context.Set(item.Key, item.Value);
            }

            var result = _engine.Execute(context);
            return Ok(new { Result = result, Context = context });
        }
    }
}
