﻿using EnterpriseIntegation.RabbitMQ;
using EnterpriseIntegration.Channels;
using EnterpriseIntegration.Message;
using EnterpriseIntegration.TestCommon;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace EnterpriseIntegration.RabbitMQ.Tests
{
    [Collection("RabbitMQ")]
    public class RabbitMQChannelFixture
    {
        private static readonly IMessageTransformer _transformer = new DefaultMessageTransformer();
        private readonly RabbitMQFixture _rabbitMQFixture;

        public record Example(string Name, int Value);

        public RabbitMQChannelFixture(RabbitMQFixture rabbitMQFixture)
        {
            _rabbitMQFixture = rabbitMQFixture;
        }

        [Fact]
        public async Task Channel_ShouldSendAndReceive()
        {
            // Arrange
            string queueName = "sendAndReceive";
            using RabbitMQChannel channel = new RabbitMQChannel(queueName, _rabbitMQFixture.ConnectionProvider, _transformer);
            IMessage<Example> message = new GenericMessage<Example>(new Example("Test", 7));
            message.MessageHeaders.Add("custom_header", "some_value");

            IMessage<Example> result = null;

            Func<IMessage<Example>, Task> subscriber = async responseMessage => result = responseMessage;

            // Act
            await channel.Subscribe(subscriber);
            await channel.Send(message);
            await TestHelper.WaitFor(() => result != null);

            // Assert
            result.Should().NotBeNull();
            result!.Payload.Should().NotBeNull();
            result.Payload.Name.Should().Be("Test");
            result.Payload.Value.Should().Be(7);

            result.MessageHeaders.Id.Should().Be(message.MessageHeaders.Id);
            result.MessageHeaders.CreatedDate.Should().Be(message.MessageHeaders.CreatedDate);
            result.MessageHeaders["custom_header"].Should().Be("some_value");
        }

        [Fact]
        public async Task Channel_ShouldSendAndReceive_SimpleType()
        {
            // Arrange
            string queueName = "sendAndReceive";
            using RabbitMQChannel channel = new RabbitMQChannel(queueName, _rabbitMQFixture.ConnectionProvider, _transformer);
            IMessage<int> message = new GenericMessage<int>(7);
            message.MessageHeaders.Add("custom_header", "some_value");

            IMessage<int> result = null;

            Func<IMessage<int>, Task> subscriber = async responseMessage => result = responseMessage;

            // Act
            await channel.Subscribe(subscriber);
            await channel.Send(message);
            await TestHelper.WaitFor(() => result != null);

            // Assert
            result.Should().NotBeNull();
            result!.Payload.Should().Be(7);

            result.MessageHeaders.Id.Should().Be(message.MessageHeaders.Id);
            result.MessageHeaders.CreatedDate.Should().Be(message.MessageHeaders.CreatedDate);
        }

        [Fact]
        public async Task Channel_ShouldSendAndReceive_Multiple()
        {
            // Arrange
            string queueName = "sendAndReceive";
            using RabbitMQChannel channel = new RabbitMQChannel(queueName, _rabbitMQFixture.ConnectionProvider, _transformer);
            List<IMessage<Example>> messages = new List<IMessage<Example>>(GenerateMessages(15));
            List<IMessage<Example>> results = new List<IMessage<Example>>();
            Func<IMessage<Example>, Task> subscriber = async responseMessage => results.Add(responseMessage);

            // Act
            await channel.Subscribe(subscriber);

            List<Task> sendingTasks = new List<Task>(messages.Count);
            foreach (var message in messages)
            {
                sendingTasks.Add(channel.Send(message));
            }
            await Task.WhenAll(sendingTasks);
            await TestHelper.WaitFor(() => results.Count == messages.Count, maxWaitTimeInMilliseconds: 10_000);

            // Assert
            results.Should().NotBeNull();
            results.Count.Should().Be(messages.Count);

            foreach (IMessage<Example> message in messages)
            {
                IMessage<Example> result = results.Find(m => m.MessageHeaders.Id == message.MessageHeaders.Id);

                result!.Payload.Should().NotBeNull();
                result.Payload.Name.Should().Be(message.Payload.Name);
                result.Payload.Value.Should().Be(message.Payload.Value);

                result.MessageHeaders.Id.Should().Be(message.MessageHeaders.Id);
                result.MessageHeaders.CreatedDate.Should().Be(message.MessageHeaders.CreatedDate);
            }
        }

        private IEnumerable<IMessage<Example>> GenerateMessages(int count = 10)
        {
            for (int i = 0; i < count; i++)
            {
                yield return new GenericMessage<Example>(new Example($"TEST_{i}", i));
            }
        }
    }
}