using Cocona;
using EnterpriseIntegration;
using EnterpriseIntegration.Components.Wiretap;
using EnterpriseIntegration.Flow;
using EnterpriseIntegration.LoadTests.Scenarios;
using EnterpriseIntegration.TestCommon.Examples;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var appBuilder = CoconaApp.CreateBuilder();

appBuilder.Services
    .AddSingleton<ServiceActivatorFlow001>()
    .AddSingleton<RoutingFlow002>()
    .AddSingleton<SplitterAggregatorFlow003>()
    .UseEnterpriseIntegration();

var app = appBuilder.Build();

app.AddCommand((ILogger<Program> logger, IWireTapService wireTapService, FlowEngine flowEngine, ScenarioEnum scenario) => {

    switch (scenario)
    {
        case ScenarioEnum.Simple: 
            SimpleScenario.CreateAndRun(logger, wireTapService, flowEngine);
            break;
    }
});

app.Run();