using Zenject;

/// <summary>
/// Bu kod Zenject bind eklenmesinden sorumludur.
/// </summary>
public class GameInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<InputFieldManager>().FromComponentInHierarchy().AsSingle();
    }
}