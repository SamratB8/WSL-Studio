namespace WslStudio.Application.Wsl;

public interface IWslDistributionParser
{
    WslDistributionParseResult ParseListVerbose(string output);
}
