using EnterpriseIntegration.Components.PreActions;
using EnterpriseIntegration.Flow;
using EnterpriseIntegration.Message;

namespace EnterpriseIntegration.Components.Wiretap;
public class WiretapService : IWireTapService, IPreAction
{
    private Dictionary<string, Dictionary<WireTapId, IWireTapService.WireTap>> _wireTaps = new();
    private Dictionary<WireTapId, string> _channelLookup = new();

    public WireTapId CreateWireTap(string channelName, IWireTapService.WireTap wireTap)
    {
        WireTapId id = WireTapId.Create();

        lock (this)
        {
            if (!_wireTaps.ContainsKey(channelName))
            {
                _wireTaps.Add(channelName, new Dictionary<WireTapId, IWireTapService.WireTap>() { { id, wireTap } });
            }
            else
            {
                _wireTaps[channelName].Add(id, wireTap);
            }

            _channelLookup.Add(id, channelName);
        }

        return id;
    }


    public async Task PreProcess(FlowNode flowNode, IMessage message)
    {
        string channelName = flowNode.InChannelName;
        if (_wireTaps.ContainsKey(channelName))
        {
            await Task.WhenAll(_wireTaps[channelName].Values.Select(wireTap => wireTap(message)));
        }
    }

    public void RemoveWireTap(WireTapId id)
    {
        string channel = _channelLookup[id];
        lock (this)
        {
            _channelLookup.Remove(id);
            _wireTaps[channel].Remove(id);
        }
    }
}