using EnterpriseIntegration.Components.Wiretap;
using EnterpriseIntegration.Flow;
using Microsoft.Extensions.Logging;

namespace EnterpriseIntegration.LoadTests.Scenarios
{
    public class SimpleScenario : NBomberBaseScenario<string>
    {
        int counter = 0;

        public SimpleScenario(ILogger logger, IWireTapService wireTapService, FlowEngine flowEngine) : base(logger, wireTapService, flowEngine)
        {
        }

        protected override string StartChannel => "001_hello";

        protected override string EndChannel => "001_end";

        protected override string GeneratePayload()
        {
            return $"Prefix{counter++}:";
        }

        public static void CreateAndRun(ILogger logger, IWireTapService wireTapService, FlowEngine flowEngine)
        {
            new SimpleScenario(logger, wireTapService, flowEngine).Run();
        }
    }
}