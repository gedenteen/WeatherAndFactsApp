using UnityEngine;
using Zenject;

public class MainInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        AppState initialState = AppState.ShowingWeather;

        Container.Bind<AppStateController>().AsSingle().WithArguments(initialState).NonLazy();
        Container.Bind<RequestsQueue>().AsSingle().NonLazy();
        Container.Bind<WeatherService>().AsSingle().NonLazy();
        Container.Bind<DogFactsService>().AsSingle().NonLazy();
    }
}