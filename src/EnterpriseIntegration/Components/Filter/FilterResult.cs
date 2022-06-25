namespace EnterpriseIntegration.Components.Filter;

/// <summary>
///     Filter Result to be returned by a method using <see cref="FilterAttribute"/>;
/// </summary>
public struct FilterResult
{
    private static readonly FilterResult DiscardValue = new(true);

    private static readonly FilterResult ForwardValue = new(false);

    private readonly bool value;

    private FilterResult(bool value)
    {
        this.value = value;
    }

    public bool ShouldDiscard => value;

    public static FilterResult Discard => DiscardValue;
    
    public static FilterResult Forward => ForwardValue;
}