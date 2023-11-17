using NetArchTest.Rules;

namespace Giraf.ArchitectureTest;

public class UnitTest1
{
    [Fact]
    public void GirafRest_Should_ReferenceGirafServices()
    {
        var result = Types.InCurrentDomain()
            .That()
            .ResideInNamespace("GirafRest")
            .Should()
            .HaveDependencyOn("GirafServices")
            .GetResult();
        
        Assert.True(result.IsSuccessful);
    }
}