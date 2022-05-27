using EnterpriseIntegration.Message;

namespace EnterpriseIntegration.Components.Splitter;

public static class MessageHeadersExtensions
{
    public const string OriginalIdKey = "eip_splitter_original_id";

    public const string MessageGroupIndex = "eip_splitter_group_index";

    public const string MessageGroupCount = "eip_splitter_group_count";

    public const string MessageGroupId = "eip_splitter_group_id";

    public static string? GetOriginalId(this IMessageHeaders headers)
    {
        return headers.GetValueOrDefault(OriginalIdKey);
    }

    public static void SetOriginalId(this IMessageHeaders headers, string? value)
    {
        SetOrRemoveHeaderField(headers, OriginalIdKey, value);
    }
    public static string? GetMessageGroupId(this IMessageHeaders headers)
    {
        return headers.GetValueOrDefault(MessageGroupId);
    }

    public static void SetMessageGroupId(this IMessageHeaders headers, string? value)
    {
        SetOrRemoveHeaderField(headers, MessageGroupId, value);
    }

    public static string? GetMessageGroupIndex(this IMessageHeaders headers)
    {
        return headers.GetValueOrDefault(MessageGroupIndex);
    }

    public static void SetMessageGroupIndex(this IMessageHeaders headers, string? value)
    {
        SetOrRemoveHeaderField(headers, MessageGroupIndex, value);
    }

    public static string? GetMessageGroupCount(this IMessageHeaders headers)
    {
        return headers.GetValueOrDefault(MessageGroupCount);
    }

    public static void SetMessageGroupCount(this IMessageHeaders headers, string? value)
    {
        SetOrRemoveHeaderField(headers, MessageGroupCount, value);
    }

    private static string? GetValueOrDefault(this IMessageHeaders headers, string fieldName) 
        => headers.ContainsKey(fieldName) ? headers[fieldName] : null;

    private static void SetOrRemoveHeaderField(IMessageHeaders headers, string fieldName, string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            if (headers.ContainsKey(fieldName))
            {
                headers.Remove(fieldName);
            }
        }
        else
        {
            headers[fieldName] = value;
        }
    }
}
