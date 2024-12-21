using UnityEngine;

[CreateAssetMenu(fileName = "ApiLinks", menuName = "ScriptableObjects/Create ApiLinks")]
public class ApiLinks : ScriptableObject
{
    [SerializeField] private string _weather = "https://api.weather.gov/gridpoints/TOP/32,81/forecast";
    [SerializeField] private string _dogFacts = "https://dogapi.dog/docs/api-v2";

    // Public read-only properties.
    // These properties provide access to the API links while preventing them 
    // from being modified in other classes, ensuring the URLs remain consistent.
    public string Weather { get { return _weather; } }
    public string DogFacts { get { return _dogFacts; } }
}
