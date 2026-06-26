namespace WslStudio.Application.Wsl;

internal static class WslKeyValueParser
{
    public static Dictionary<string, string> Parse(string output)
    {
        Dictionary<string, string> values = new(StringComparer.OrdinalIgnoreCase);

        foreach (string rawLine in output.Replace("\0", string.Empty, StringComparison.Ordinal).Split('\n'))
        {
            string line = rawLine.Trim();
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            int separatorIndex = line.IndexOf(':', StringComparison.Ordinal);
            if (separatorIndex <= 0 || separatorIndex >= line.Length - 1)
            {
                continue;
            }

            string key = line[..separatorIndex].Trim();
            string value = line[(separatorIndex + 1)..].Trim();

            if (!string.IsNullOrWhiteSpace(key) && !string.IsNullOrWhiteSpace(value))
            {
                values[key] = value;
            }
        }

        return values;
    }
}
