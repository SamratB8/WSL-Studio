using System.Reflection;

namespace WslStudio.Tests;

public sealed class SolutionStructureTests
{
    [Theory]
    [InlineData("WslStudio.Core")]
    [InlineData("WslStudio.Application")]
    [InlineData("WslStudio.Infrastructure")]
    public void LayerAssemblyCanBeLoaded(string assemblyName)
    {
        Assembly assembly = Assembly.Load(assemblyName);

        Assert.Equal(assemblyName, assembly.GetName().Name);
    }
}
