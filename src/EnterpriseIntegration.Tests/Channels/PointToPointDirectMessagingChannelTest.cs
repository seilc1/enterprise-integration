using EnterpriseIntegration.Channels;
using EnterpriseIntegration.Message;
using FluentAssertions;
using Xunit;

namespace EnterpriseIntegration.Tests.Channels
{
    public class PointToPointDirectMessagingChannelTest
    {

        [Fact]
        void ShouldReflectType()
        {
            // arrange
            var stringValue = "Woop";
            var msg = new GenericMessage<string>(stringValue);

            // act
            var result = IMessage<object>.ReflectPayloadType(msg);

            // assert
            result.Should().Be(typeof(string));
        }

        [Fact]
        void ShouldTransformMessageCorrectly()
        {
            // arrange
            var stringValue = "Woop";
            var msg = new GenericMessage<string>(stringValue);

            // act
            var result = (new DefaultMessageTransformer()).TransformMessage<object>(msg);

            // assert
            result.Payload.Should().BeEquivalentTo(stringValue);
            result.PayloadType.Should().Be(typeof(string));
            result.MessageHeaders.Id.Should().BeEquivalentTo(msg.MessageHeaders.Id);
        }
    }
}
