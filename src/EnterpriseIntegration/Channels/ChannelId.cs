namespace EnterpriseIntegration.Channels;

[StronglyTypedId(backingType: StronglyTypedIdBackingType.String, jsonConverter: StronglyTypedIdJsonConverter.SystemTextJson)]
public partial struct ChannelId
{
    public static implicit operator ChannelId(string value)
    {
        return new ChannelId(value);
    }
}