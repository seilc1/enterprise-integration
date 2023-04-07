using EnterpriseIntegration.Components.Wiretap;
using EnterpriseIntegration.Flow;
using EnterpriseIntegration.Message;
using Microsoft.Extensions.Logging;
using NBomber.Contracts;
using NBomber.CSharp;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace EnterpriseIntegration.LoadTests.Scenarios
{
    public abstract class NBomberBaseScenario<T>
    {
        protected abstract string StartChannel { get; }

        protected abstract string EndChannel { get; }

        protected int CheckIntervalInMilliseconds { get; set; } = 0;

        protected int MaxWaitTimeInMilliseconds { get; set; } = 20;

        protected abstract T GeneratePayload();

        protected ILogger Logger { get; }

        protected IWireTapService WireTapService { get; }

        protected FlowEngine FlowEngine { get; }

        private readonly ConcurrentDictionary<string, string> _receivedMessages = new ();

        protected NBomberBaseScenario(ILogger logger, IWireTapService wireTapService, FlowEngine flowEngine)
        {
            Logger = logger;
            WireTapService = wireTapService;
            FlowEngine = flowEngine;
        }

        public void Run()
        {
            WireTapId wireTapId = WireTapService.CreateWireTap(EndChannel, MessageWireTap);

            var scenario = Scenario.Create("SendAndWaitMessage", async context =>
                {
                    var message = new GenericMessage<T>(GeneratePayload());
                    string messageId = message.MessageHeaders.Id;

                    await FlowEngine.Submit(StartChannel, message);
                    await WaitFor(() => HasMessageReceived(messageId), CheckIntervalInMilliseconds, MaxWaitTimeInMilliseconds);

                    return HasMessageReceived(messageId) ? Response.Ok() : Response.Fail();
                })
                .WithWarmUpDuration(TimeSpan.FromSeconds(1))
                .WithLoadSimulations(new [] { LoadSimulation.NewKeepConstant(Environment.ProcessorCount, TimeSpan.FromSeconds(60)) });

            NBomberRunner.RegisterScenarios(scenario).Run();
            WireTapService.RemoveWireTap(wireTapId);
        }

        private Task MessageWireTap(IMessage message)
        {
            _receivedMessages.TryAdd(message.MessageHeaders.Id, "received");
            return Task.CompletedTask;
        }

        protected bool HasMessageReceived(string messageId) =>_receivedMessages.ContainsKey(messageId);

        public static async Task WaitFor(Func<bool> condition, int checkIntervalInMilliseconds = 1, int maxWaitTimeInMilliseconds = 500)
        {
            Stopwatch watch = Stopwatch.StartNew();
            while (!condition() && watch.ElapsedMilliseconds < maxWaitTimeInMilliseconds)
            {
                await Task.Delay(checkIntervalInMilliseconds);
            }
        }
    }
}