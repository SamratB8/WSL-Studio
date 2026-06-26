namespace WslStudio.Application.Wsl;

public interface IWslStatusParser
{
    WslStatusParseResult Parse(string output);
}
