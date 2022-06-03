using Ductus.FluentDocker.Services;
using EnterpriseIntegation.RabbitMQ;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using Xunit;

namespace EnterpriseIntegration.RabbitMQ.Tests
{
    public abstract class RabbitMQFixture : IDisposable
    {
        protected IContainerService Container { get; private set; }

        private const string RabbitMQUser = "rabbit_mq_test";
        private const string RabbitMQPassword = "rabbit_mq_p@ssw0rd";

        protected RabbitMQSettings Settings => new RabbitMQSettings
        {
            Hostname = "127.0.0.1",
            Username = RabbitMQUser,
            Password = RabbitMQPassword
        };

        protected ConnectionFactory ConnectionFactory { get; init; }

        protected RabbitMQFixture()
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
            if (_connection != null)
            {
                _connection.Dispose();
            }

            Container.Dispose();
        }

        private IConnection? _connection;

        protected IConnection Connection => _connection ?? (_connection = ConnectionFactory.CreateConnection());
    }
}