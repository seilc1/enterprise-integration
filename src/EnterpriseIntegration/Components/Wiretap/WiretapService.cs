using EnterpriseIntegration.Channels;
using EnterpriseIntegration.Components.PreActions;
using EnterpriseIntegration.Flow;
using EnterpriseIntegration.Message;

namespace EnterpriseIntegration.Components.Wiretap;
public class WiretapService : IWireTapService, IPreAction
{
    private Dictionary<ChannelId, Dictionary<WireTapId, IWireTapService.WireTap>> _wireTaps = new();
    private Dictionary<WireTapId, ChannelId> _channelLookup = new();

    public WireTapId CreateWireTap(ChannelId channelId, IWireTapService.WireTap wireTap)
    {
        WireTapId id = WireTapId.Create();

        lock (this)
        {
            if (!_wireTaps.ContainsKey(channelId))
            {
                _wireTaps.Add(channelId, new Dictionary<WireTapId, IWireTapService.WireTap>() { { id, wireTap } });
            }
            else
            {
                _wireTaps[channelId].Add(id, wireTap);
            }

            _channelLookup.Add(id, channelId);
        }

        return id;
    }


    public async Task PreProcess(FlowNode flowNode, IMessage message)
    {
        ChannelId channelId = flowNode.InChannelId;
        if (_wireTaps.ContainsKey(channelId))
        {
            await Task.WhenAll(_wireTaps[channelId].Values.Select(wireTap => wireTap(message)));
        }
    }

    public void RemoveWireTap(WireTapId id)
    {
        ChannelId channel = _channelLookup[id];
        lock (this)
        {
            _channelLookup.Remove(id);
            _wireTaps[channel].Remove(id);
        }
    }
}