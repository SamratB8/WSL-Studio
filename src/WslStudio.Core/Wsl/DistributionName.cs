namespace WslStudio.Core.Wsl;

public sealed record DistributionName
{
    private DistributionName(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static bool TryCreate(string? value, out DistributionName? distributionName)
    {
        distributionName = null;

        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        string trimmedValue = value.Trim();

        if (trimmedValue.IndexOfAny(['\0', '\r', '\n']) >= 0)
        {
            return false;
        }

        distributionName = new DistributionName(trimmedValue);
        return true;
    }

    public override string ToString()
    {
        return Value;
    }
}
