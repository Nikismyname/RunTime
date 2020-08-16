using Zenject;

public class ZenInstaller : MonoInstaller<ZenInstaller>
{
    public override void InstallBindings()
    {
        this.Container.Bind<SpellcraftProcUI>().AsSingle();
        this.Container.Bind<SelectableButtons>().AsTransient();
    }
}