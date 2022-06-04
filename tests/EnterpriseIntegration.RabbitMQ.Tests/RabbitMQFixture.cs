using Ductus.FluentDocker.Services;
using EnterpriseIntegation.RabbitMQ;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System;

namespace EnterpriseIntegration.RabbitMQ.Tests
{
    public sealed class RabbitMQFixture : IDisposable
    {
        private IContainerService Container { get; set; }

        private const string RabbitMQUser = "rabbit_mq_test";

        private const string RabbitMQPassword = "rabbit_mq_p@ssw0rd";

        public RabbitMQSettings Settings => new RabbitMQSettings
        {
            Hostname = "127.0.0.1",
            Username = RabbitMQUser,
            Password = RabbitMQPassword
        };

        private ConnectionFactory ConnectionFactory { get; init; }

        public RabbitMQFixture()
        {
            Container = new Ductus.FluentDocker.Builders.Builder()
                .UseContainer()
                .UseImage("rabbitmq")
                .WithHostName("rabbitmq")
                .WithEnvironment($"RABBITMQ_DEFAULT_USER={RabbitMQUser}", $"RABBITMQ_DEFAULT_PASS={RabbitMQPassword}")
                .ExposePort(5672, 5672)
                .ExposePort(15672, 15672)
                .WaitForMessageInLog("Server startup complete", TimeSpan.FromSeconds(30))
                .Build()
                .Start();

            ConnectionFactory = new ConnectionFactory() { HostName = Settings.Hostname, UserName = Settings.Username, Password = Settings.Password };
        }

        public void Dispose()
        {
            if (_rabbitMQConnectionProvider != null)
            {
                _rabbitMQConnectionProvider.Dispose();
            }


            Container.Dispose();
        }

        private IRabbitMQConnectionProvider? _rabbitMQConnectionProvider;

        public IRabbitMQConnectionProvider ConnectionProvider => _rabbitMQConnectionProvider ?? (_rabbitMQConnectionProvider = new RabbitMQConnectionProvider(new OptionsWrapper<RabbitMQSettings>(Settings)));
    }
}