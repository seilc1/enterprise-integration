using EnterpriseIntegration.Channels;
using EnterpriseIntegration.Flow;
using EnterpriseIntegration.Message;
using EnterpriseIntegration.TestCommon;
using EnterpriseIntegration.TestCommon.Examples;
using FluentAssertions;
using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace EnterpriseIntegration.IntegrationTests.Flows
{
    public class ErrorFlowFixture
    {
        private readonly ITestOutputHelper _testOutputHelper;

        private readonly IMessageGateway _messageGateway;
        private readonly ErrorFlow _errorFlow;

        public ErrorFlowFixture(ITestOutputHelper testOutputHelper, IMessageGateway messageGateway, ErrorFlow errorFlow)
        {
            _testOutputHelper = testOutputHelper;
            _messageGateway = messageGateway;
            _errorFlow = errorFlow;
        }

        [Fact]
        public async Task SendMessage_ShouldProduceExceptionToErrorChannel()
        {
            // Arrange
            IMessageHeaders headers = new MessageHeaders();
            headers.WithErrorChannel(ErrorFlow.ErrorChannelName);
            IMessage message = new GenericMessage<ExamplePayload>(headers, ExamplePayload.CreateRandom());
            _errorFlow.ResetLastMessageFailure();

            // Act
            await _messageGateway.SendMessage("error_exception", message);
            await TestHelper.WaitFor(() => _errorFlow.LastMessageFailure != null);

            // Assert
            _errorFlow.LastMessageFailure!.Exception.Should().BeOfType<ArgumentException>().Which.Message.Should().Be("Fails because of an Argument");
        }

        [Fact]
        public async Task SendMessage_ShouldProduceExceptionToErrorChannel_WhenUsingIncorrectData()
        {
            // Arrange
            IMessageHeaders headers = new MessageHeaders();
            headers.WithErrorChannel(ErrorFlow.ErrorChannelName);
            IMessage message = new GenericMessage<ExamplePayload>(headers, ExamplePayload.CreateRandom());
            _errorFlow.ResetLastMessageFailure();

            // Act
            await _messageGateway.SendMessage("error_with_type", message);
            await TestHelper.WaitFor(() => _errorFlow.LastMessageFailure != null);

            // Assert
            PayloadTransformationException error = _errorFlow.LastMessageFailure!.Exception.Should().BeOfType<PayloadTransformationException>().Which;
            error.ChannelName.Should().Be("error_exception", "fails by transitioning from `error_with_type` to `error_exception`");
            error.ReceivedType.Should().Be(typeof(double));
            error.MethodParameterType.Should().Be(typeof(ExamplePayload));
        }

        [Fact]
        public async Task SendMessage_ShouldThrow_WhenWrongTypeForFirstChannel()
        {
            // Arrange
            IMessageHeaders headers = new MessageHeaders();
            headers.WithErrorChannel(ErrorFlow.ErrorChannelName);
            IMessage message = new GenericMessage<int>(headers, 12);
            _errorFlow.ResetLastMessageFailure();

            Action act = () => _messageGateway.SendMessage("error_exception", message).Wait();

            // Act
            PayloadTransformationException error = act.Should().Throw<PayloadTransformationException>().Which;

            // Assert
            _errorFlow.LastMessageFailure.Should().BeNull(); 
            error.ReceivedType.Should().Be(typeof(int));
            error.MethodParameterType.Should().Be(typeof(ExamplePayload));
        }
    }
}