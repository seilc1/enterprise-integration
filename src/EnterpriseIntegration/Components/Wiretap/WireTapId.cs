namespace EnterpriseIntegration.Components.Wiretap;

[StronglyTypedId(jsonConverter: StronglyTypedIdJsonConverter.SystemTextJson)]
public partial struct WireTapId {

    public static WireTapId Create()
    {
        return new WireTapId(Guid.NewGuid());
    }
}