using EnterpriseIntegration.Message;

namespace EnterpriseIntegration.Components.History;

public static class MessageHeadersExtensions
{
    public const string HistoryItemCountKey = "EI_HistoryItemCount";

    public const string HistoryItemPrefix = "EI_HistoryItem_";

    public const string HistoryItemFormat = HistoryItemPrefix + "{0}";

    public static int GetHistoryItemCount(this IMessageHeaders messageHeaders)
    {
        if (messageHeaders.TryGetValue(HistoryItemCountKey, out string? historyItemCount))
        {
            return int.Parse(historyItemCount);
        }

        return 0;
    }

    public static void SetHistoryItemCount(this IMessageHeaders messageHeaders, int count)
        => messageHeaders[HistoryItemCountKey] = count.ToString();

    public static void PushHistoryItem(this IMessageHeaders messageHeaders, string flowNodeInfo)
    {
        int currentIndex = GetHistoryItemCount(messageHeaders);

        messageHeaders[string.Format(HistoryItemFormat, currentIndex)] = flowNodeInfo;
        SetHistoryItemCount(messageHeaders, ++currentIndex);
    }

    public static IEnumerable<string> GetHistoryItems(this IMessageHeaders messageHeaders)
        => messageHeaders.Where(h => h.Key.StartsWith(HistoryItemPrefix)).OrderBy(h => h.Key).Select(h => h.Value);
}