using System;
using System.Collections.Generic;
using Newtonsoft.Json;

public class WeatherResponse
{
    [JsonProperty("properties")]
    public WeatherProperties Properties { get; set; }
}

public class WeatherProperties
{
    [JsonProperty("periods")]
    public List<WeatherPeriod> Periods { get; set; }
}

public class WeatherPeriod
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("startTime")]
    public DateTime StartTime { get; set; }

    [JsonProperty("endTime")]
    public DateTime EndTime { get; set; }

    [JsonProperty("isDaytime")]
    public bool IsDaytime { get; set; }

    [JsonProperty("temperature")]
    public int Temperature { get; set; }

    [JsonProperty("temperatureUnit")]
    public string TemperatureUnit { get; set; }

    [JsonProperty("icon")]
    public string Icon { get; set; }

    [JsonProperty("shortForecast")]
    public string ShortForecast { get; set; }

    [JsonProperty("detailedForecast")]
    public string DetailedForecast { get; set; }
}
