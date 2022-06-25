using EnterpriseIntegration.Flow;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using System.Linq;
using Xunit;

namespace EnterpriseIntegration.Tests.Flow
{
    public class AttributeFlowDataSourceTest
    {
        private readonly ILogger<AttributeFlowDataSource> _logger = NSubstitute.Substitute.For<ILogger<AttributeFlowDataSource>>();
        private readonly AttributeFlowDataSource sut;

        public AttributeFlowDataSourceTest()
        {
            sut = new AttributeFlowDataSource(_logger);
        }

        [Fact]
        public void ShouldGetMethodsFromExample001()
        {
            var result = AttributeFlowDataSource.GetAllMethodsWithIntegrationAttribute().ToList();

            result.Count.Should().BeGreaterThanOrEqualTo(4);
            result.Should().ContainSingle(m => m.DeclaringType!.Name == "ExampleFlow001" && m.Name == "Hello");
            result.Should().ContainSingle(m => m.DeclaringType!.Name == "ExampleFlow001" && m.Name == "World");
            result.Should().ContainSingle(m => m.DeclaringType!.Name == "ExampleFlow001" && m.Name == "Randomizer");
            result.Should().ContainSingle(m => m.DeclaringType!.Name == "ExampleFlow001" && m.Name == "End");
        }

        [Fact]
        public void ShouldGetFlowNodesFromExample001()
        {
            var result = sut.GetAllFlowNodes().ToList();

            result.Count.Should().BeGreaterThanOrEqualTo(4);
            result.Should().ContainSingle(n => n.InChannelId == "hello" && n.OutChannelId == "world" && n.NodeType == FlowNodeType.ServiceActivator);
            result.Should().ContainSingle(n => n.InChannelId == "world" && n.OutChannelId == "random" && n.NodeType == FlowNodeType.ServiceActivator);
            result.Should().ContainSingle(n => n.InChannelId == "random" && n.OutChannelId == null && n.NodeType == FlowNodeType.Router);
            result.Should().ContainSingle(n => n.InChannelId == "end" && n.OutChannelId == null && n.NodeType == FlowNodeType.Endpoint);
        }

    }
}
