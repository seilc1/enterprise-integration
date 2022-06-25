using EnterpriseIntegration.Attributes;
using EnterpriseIntegration.Components.Filter;
using Microsoft.Extensions.Logging;

namespace EnterpriseIntegration.TestCommon.Examples
{
    public class FilterFlow
    {
        private readonly ILogger<FilterFlow> _logger;

        public record FilterMessage(bool ShouldFilter);

        public FilterFlow(ILogger<FilterFlow> logger)
        {
            _logger = logger;
        }

        [Filter("filter", "filter-end")]
        public FilterResult Filter(FilterMessage message)
        {
            return message.ShouldFilter ? FilterResult.Discard : FilterResult.Forward;
        }

        [Endpoint(inChannelId: "filter-end")]
        public void End(FilterMessage message)
        {
            _logger.LogInformation("Received Message: {Message}", message);
        }
    }
}