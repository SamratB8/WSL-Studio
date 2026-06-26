namespace WslStudio.Tests.Application.Wsl;

internal static class WslListVerboseFixtures
{
    public const string NormalOutput = """
      NAME            STATE           VERSION
    * Ubuntu          Running         2
      Debian          Stopped         2
    """;

    public const string NameWithSpacesOutput = """
      NAME                    STATE           VERSION
      Ubuntu Preview          Stopped         2
    """;

    public const string NullSeparatedOutput =
        " \0 \0N\0A\0M\0E\0 \0 \0 \0 \0 \0 \0 \0 \0 \0S\0T\0A\0T\0E\0 \0 \0 \0 \0 \0 \0 \0 \0 \0V\0E\0R\0S\0I\0O\0N\0\r\0\n\0*\0 \0U\0b\0u\0n\0t\0u\0 \0 \0 \0 \0 \0 \0 \0 \0 \0R\0u\0n\0n\0i\0n\0g\0 \0 \0 \0 \0 \0 \0 \0 \02\0\r\0\n\0";
}
