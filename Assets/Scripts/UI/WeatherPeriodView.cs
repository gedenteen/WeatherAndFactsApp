using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class WeatherPeriodView : MonoBehaviour
{
    [SerializeField] private Image _weatherIcon;
    [SerializeField] private TextMeshProUGUI _periodNameText;
    [SerializeField] private TextMeshProUGUI _temperatureText;

    public async UniTask SetData(WeatherPeriod weatherPeriod, WeatherDataService weatherDataService)
    {
        _periodNameText.text = weatherPeriod.Name;
        _temperatureText.text = $"{weatherPeriod.Temperature}°{weatherPeriod.TemperatureUnit}";
        _weatherIcon.sprite = await weatherDataService.GetIcon(weatherPeriod.Icon);
    }
}
