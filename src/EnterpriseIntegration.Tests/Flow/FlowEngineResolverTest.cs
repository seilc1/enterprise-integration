using EnterpriseIntegration.Flow;
using EnterpriseIntegration.Message;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit;

namespace EnterpriseIntegration.Tests.Flow
{
    public class FlowEngineResolverTest
    {
        public class GoodCaseFlow {
            public void NoType() { }

            public void SimpleStringType(string payload) { }

            public void ComplexExampleFlowType(GoodCaseFlow exampleFlow) { }

            public void WrappedSimpleDoubleType(IMessage<double> message) { }

            public void WrappedComplexListType(GenericMessage<List<string>> message) { }

            public void WrappedComplexListTypeWithHeaders(IMessageHeaders headers, IMessage<List<string>> message) { }
        }

        [Theory]
        [InlineData("NoType", typeof(VoidParameter))]
        [InlineData("SimpleStringType", typeof(string))]
        [InlineData("ComplexExampleFlowType", typeof(GoodCaseFlow))]
        [InlineData("WrappedSimpleDoubleType", typeof(double))]
        [InlineData("WrappedComplexListType", typeof(List<string>))]
        [InlineData("WrappedComplexListTypeWithHeaders", typeof(List<string>))]
        public void ShouldFindPayloadType(string methodName, Type expectedType)
        {
            // arrange
            Type parent = typeof(GoodCaseFlow);
            MethodInfo methodInfo = parent.GetMethod(methodName)!;

            FlowNode node = new FlowNode("TEST", FlowNodeType.Undefined, methodName, methodInfo, null);

            // act
            Type result = FlowEngineResolver.ExpectedPayloadType(node);

            // assert
            result.Should().Be(expectedType);
        }

        public class BadCaseFlow
        {
            public void DoubleSimpleParameter(string payload1, string payload2) { }
            public void MixParameters(string payload1, IMessage<string> payload2) { }
            public void MixParametersWithHeaders(IMessage<string> payload2, IMessageHeaders headers, string payload1) { }
        }

        [Theory]
        [InlineData("DoubleSimpleParameter", "TooManyPayloadParameters: FlowNode:TEST with bound method:BadCaseFlow.DoubleSimpleParameter has too many payload parameters defined.")]
        [InlineData("MixParameters", "TooManyPayloadParameters: FlowNode:TEST with bound method:BadCaseFlow.MixParameters has too many payload parameters defined.")]
        [InlineData("MixParametersWithHeaders", "TooManyPayloadParameters: FlowNode:TEST with bound method:BadCaseFlow.MixParametersWithHeaders has too many payload parameters defined.")]
        public void ShouldThrowForIncorrectMethodDefinition(string methodName, string exceptionMessage)
        {
            // arrange
            Type parent = typeof(BadCaseFlow);
            MethodInfo methodInfo = parent.GetMethod(methodName);

            FlowNode node = new FlowNode("TEST", FlowNodeType.Undefined, methodName, methodInfo, null);

            // act
            var exception = Assert.Throws<FlowNodeMethodInvalidException>(() => FlowEngineResolver.ExpectedPayloadType(node));

            // assert
            exception.Message.Should().Be(exceptionMessage);
        }
    }
}
