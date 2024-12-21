using UnityEngine;
using Zenject;

public class MainInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<RequestsQueue>().AsSingle().NonLazy();
        Container.Bind<WeatherService>().AsSingle().NonLazy();
    }
}