using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Zenject;

public class DogBreedView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _id;
    [SerializeField] private TextMeshProUGUI _name;
    [SerializeField] private Button _button;

    private DogsInfoUiController _dogsInfoUiController;
    private int _index = -1;

    private void Awake()
    {
        _button.onClick.AddListener(OnButtonClick);
    }

    private void OnDestroy()
    {
        _button.onClick.RemoveListener(OnButtonClick);
    }

    private void OnButtonClick()
    {
        if (_dogsInfoUiController != null)
        {
            _dogsInfoUiController.ShowPopUpWithInfo(_index);
        }
        else
        {
            Debug.LogError($"DogBreedView: OnButtonClick: _dogsInfoUiController is null");
        }
    }

    public void SetData(int id, string name, DogsInfoUiController controller)
    {
        _dogsInfoUiController = controller;
        _index = id;
        _id.text = (id + 1).ToString();
        _name.text = name;
    }
}
