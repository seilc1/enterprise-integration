using EnterpriseIntegration.Flow;
using FluentAssertions;
using System.Linq;
using Xunit;

namespace EnterpriseIntegration.Tests.Flow
{
    public class AttributeFlowDataSourceTest
    {
        AttributeFlowDataSource sut = new AttributeFlowDataSource();

        [Fact]
        public void ShouldGetMethodsFromExample001()
        {
            var result = sut.GetAllMethodsWithIntegrationAttribute().ToList();

            result.Count().Should().BeGreaterThanOrEqualTo(4);
            result.Should().ContainSingle(m => m.DeclaringType!.Name == "ExampleFlow001" && m.Name == "Hello");
            result.Should().ContainSingle(m => m.DeclaringType!.Name == "ExampleFlow001" && m.Name == "World");
            result.Should().ContainSingle(m => m.DeclaringType!.Name == "ExampleFlow001" && m.Name == "Randomizer");
            result.Should().ContainSingle(m => m.DeclaringType!.Name == "ExampleFlow001" && m.Name == "End");
        }

        [Fact]
        public void ShouldGetFlowNodesFromExample001()
        {
            var result = sut.GetAllFlowNodes().ToList();

            result.Count().Should().BeGreaterThanOrEqualTo(4);
            result.Should().ContainSingle(n => n.InChannelName == "hello" && n.OutChannelName == "world" && n.NodeType == FlowNodeType.ServiceActivator);
            result.Should().ContainSingle(n => n.InChannelName == "world" && n.OutChannelName == "random" && n.NodeType == FlowNodeType.ServiceActivator);
            result.Should().ContainSingle(n => n.InChannelName == "random" && n.OutChannelName == null && n.NodeType == FlowNodeType.Router);
            result.Should().ContainSingle(n => n.InChannelName == "end" && n.OutChannelName == null && n.NodeType == FlowNodeType.Endpoint);
        }

    }
}
