using EnterpriseIntegration.Message;

namespace EnterpriseIntegration.Components.Wiretap;
public interface IWireTapService
{
    public delegate Task WireTap(IMessage message);

    /// <summary>
    ///     Creates a new wire tap for a given channel.
    /// </summary>
    /// <param name="channelName">Name of the channel.</param>
    /// <param name="wireTap">Delegate  used to pass the <see cref="IMessage"/>.</param>
    /// <returns>Id of the WireTap to be removed, after completion.</returns>
    public WireTapId CreateWireTap(string channelName, WireTap wireTap);

    /// <summary>
    ///     Removes a wire tap.
    /// </summary>
    /// <param name="id">Id identifying the WireTap.</param>
    public void RemoveWireTap(WireTapId id);
}