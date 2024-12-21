using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class WeatherPeriodView : MonoBehaviour
{
    [SerializeField] private Image _weatherIcon;
    [SerializeField] private TextMeshProUGUI _periodNameText;
    [SerializeField] private TextMeshProUGUI _temperatureText;

    public void SetData(WeatherPeriod weatherPeriod)
    {
        _periodNameText.text = weatherPeriod.Name;
        _temperatureText.text = $"{weatherPeriod.Temperature}°F";
        StartCoroutine(LoadIcon(weatherPeriod.Icon));
    }

    private IEnumerator LoadIcon(string url)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Texture2D texture = DownloadHandlerTexture.GetContent(request);
            _weatherIcon.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
        }
        else
        {
            Debug.LogError($"Failed to load weather icon: {request.error}");
        }
    }
}
