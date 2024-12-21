using System;
using System.Collections.Generic;
using Newtonsoft.Json;

[Serializable]
public class DogApiResponse
{
    [JsonProperty("data")]
    public List<DogBreedData> Data { get; set; }

    [JsonProperty("links")]
    public DogApiLinks Links { get; set; }
}

[Serializable]
public class DogBreedData
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("attributes")]
    public DogAttributes Attributes { get; set; }

    [JsonProperty("relationships")]
    public DogRelationships Relationships { get; set; }
}

[Serializable]
public class DogAttributes
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("description")]
    public string Description { get; set; }

    [JsonProperty("life")]
    public LifeSpan Life { get; set; }

    [JsonProperty("male_weight")]
    public WeightRange MaleWeight { get; set; }

    [JsonProperty("female_weight")]
    public WeightRange FemaleWeight { get; set; }

    [JsonProperty("hypoallergenic")]
    public bool Hypoallergenic { get; set; }
}

[Serializable]
public class LifeSpan
{
    [JsonProperty("max")]
    public int Max { get; set; }

    [JsonProperty("min")]
    public int Min { get; set; }
}

[Serializable]
public class WeightRange
{
    [JsonProperty("max")]
    public int Max { get; set; }

    [JsonProperty("min")]
    public int Min { get; set; }
}

[Serializable]
public class DogRelationships
{
    [JsonProperty("group")]
    public DogGroup Group { get; set; }
}

[Serializable]
public class DogGroup
{
    [JsonProperty("data")]
    public DogGroupData Data { get; set; }
}

[Serializable]
public class DogGroupData
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("type")]
    public string Type { get; set; }
}

[Serializable]
public class DogApiLinks
{
    [JsonProperty("self")]
    public string Self { get; set; }

    [JsonProperty("current")]
    public string Current { get; set; }

    [JsonProperty("next")]
    public string Next { get; set; }

    [JsonProperty("last")]
    public string Last { get; set; }
}
