using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "ConfigsInstaller", menuName = "Installers/ConfigsInstaller")]
public class ConfigsInstaller : ScriptableObjectInstaller<ConfigsInstaller>
{
    [SerializeField] private ApiLinks _apiLinks;
    [SerializeField] private Timings _timings;

    public override void InstallBindings()
    {
        Container.BindInstance(_apiLinks).AsSingle();
        Container.BindInstance(_timings).AsSingle();
    }
}
