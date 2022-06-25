using EnterpriseIntegration.Errors;
using EnterpriseIntegration.Message;

namespace EnterpriseIntegration.Flow.MessageProcessing;

public abstract class InvokingMessageProcessor : IMessageProcessor
{
    protected IServiceProvider ServiceProvider { get; }

    protected InvokingMessageProcessor(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
    }

    public abstract FlowNodeType HandledType { get; }

    public abstract Task<IEnumerable<IMessage>> Process<T>(IMessage<T> message, FlowNode flowNode, SendMessageAsync messageSender);


    protected static IMessage AsMessage(IMessage initialMessage, object? payload)
    {
        if (payload is null)
        {
            throw new ArgumentNullException(nameof(payload));
        }

        if (payload.GetType().IsMessage())
        {
            return (IMessage)payload;
        }

        return GenericMessage<object>.From(initialMessage, payload);
    }

    protected object? InvokeFlowNodeMethod<T>(IMessage<T> message, FlowNode flowNode)
    {
        var parent = GetInstantiatedClassDeclaringMethod(flowNode);

        var parameterInfos = flowNode.MethodInfo.GetParameters();
        var parameters = new object?[parameterInfos.Length];
        foreach (var parameterInfo in parameterInfos)
        {
            if (parameterInfo.ParameterType == typeof(T))
            {
                parameters[parameterInfo.Position] = message.Payload;
            }
            else if (parameterInfo.ParameterType == typeof(IMessage<T>))
            {
                parameters[parameterInfo.Position] = message;
            }
            else if (parameterInfo.ParameterType == typeof(IMessageHeaders))
            {
                parameters[parameterInfo.Position] = message.MessageHeaders;
            }
            else
            {
                throw new EnterpriseIntegrationException($"FlowNode:{flowNode.Name} on {flowNode.MethodInfo.DeclaringType}.{flowNode.MethodInfo.Name} has an unsupported Parameter: {parameterInfo.ParameterType} {parameterInfo.Name}");
            }
        }

        return flowNode.MethodInfo.Invoke(parent, parameters);
    }

    protected object GetInstantiatedClassDeclaringMethod(FlowNode flowNode)
    {
        if (flowNode.MethodInfo.DeclaringType == null)
        {
            throw new EnterpriseIntegrationException($"FlowNode:{flowNode.Name}'s Method:{flowNode.MethodInfo.Name} must be defined on a class");
        }

        var parent = ServiceProvider.GetService(flowNode.MethodInfo.DeclaringType);
        if (parent == null)
        {
            throw new EnterpriseIntegrationException($"FlowNode:{flowNode.Name}' uses Method:{flowNode.MethodInfo.Name} on Class:{flowNode.MethodInfo.DeclaringType}. No instance of Class:{flowNode.MethodInfo.DeclaringType} is registered in the service provider.");
        }

        return parent;
    }
}
