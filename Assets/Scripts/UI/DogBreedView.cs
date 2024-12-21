using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DogBreedView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _id;
    [SerializeField] private TextMeshProUGUI _name;

    public void SetData(int id, string name)
    {
        _id.text = id.ToString();
        _name.text = name;
    }
}
