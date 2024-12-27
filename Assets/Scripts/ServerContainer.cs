using JamForge.Resolver;
using VContainer;

public class TestServices
{
    public string GetText() => "Hello World";
}

public class ServerContainer : JamInstaller
{
    public override void Install(IContainerBuilder builder)
    {
        builder.Register<TestServices>(Lifetime.Singleton);
    }
}
