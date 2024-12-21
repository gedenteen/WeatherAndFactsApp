using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Timings", menuName = "ScriptableObjects/Create Timings")]
public class Timings : ScriptableObject
{
    [SerializeField] private float _secondsForUpdateWeatherData = 5f;

    // Public read-only properties.
    // These properties provide access to the API links while preventing them 
    // from being modified in other classes, ensuring the URLs remain consistent.
    public float SecondsForUpdateWeatherData { get { return _secondsForUpdateWeatherData; } }
}
