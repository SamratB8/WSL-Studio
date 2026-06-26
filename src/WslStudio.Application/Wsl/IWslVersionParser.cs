namespace WslStudio.Application.Wsl;

public interface IWslVersionParser
{
    WslVersionParseResult Parse(string output);
}
