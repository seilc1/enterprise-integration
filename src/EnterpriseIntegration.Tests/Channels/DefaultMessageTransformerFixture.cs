using EnterpriseIntegration.Channels;
using FluentAssertions;
using Xunit;

namespace EnterpriseIntegration.Tests.Channels
{
    public class DefaultMessageTransformerFixture
    {
        public record ExampleRecord(string Name, int value);


        [Fact]
        public void TransformPayload_ShouldTransformObject_ToRecord()
        {
            // Arrange
            object payload = new ExampleRecord("Test", 0);

            // Act
            ExampleRecord? result = DefaultMessageTransformer.TransformPayload<ExampleRecord>(payload);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<ExampleRecord>();
            result!.Name.Should().Be("Test");
            result.value.Should().Be(0);
        }

        [Fact]
        public void TransformPayload_ShouldTransformObject_ToObject()
        {
            // Arrange
            object payload = new ExampleRecord("Test", 0);

            // Act
            object? result = DefaultMessageTransformer.TransformPayload<object>(payload);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<ExampleRecord>();
            ((ExampleRecord)result).Name.Should().Be("Test");
            ((ExampleRecord)result).value.Should().Be(0);
        }

        [Fact]
        public void TransformPayload_ShouldTransformObject_ToString()
        {
            // Arrange
            object payload = "Some Text";

            // Act
            string? result = DefaultMessageTransformer.TransformPayload<string>(payload);

            // Assert
            result.Should().NotBeNull();
            result.Should().Be("Some Text");
        }

        [Fact]
        public void TransformPayload_ShouldTransformObject_ToInt()
        {
            // Arrange
            object payload = 12_000;

            // Act
            int? result = DefaultMessageTransformer.TransformPayload<int>(payload);

            // Assert
            result.Should().NotBeNull();
            result.Should().Be(12_000);
        }

        [Fact]
        public void TransformPayload_ShouldTransformObject_ToDouble()
        {
            // Arrange
            object payload = 12.05;

            // Act
            double? result = DefaultMessageTransformer.TransformPayload<double>(payload);

            // Assert
            result.Should().NotBeNull();
            result.Should().Be(12.05);
        }
    }
}