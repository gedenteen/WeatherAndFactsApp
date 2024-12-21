using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] private Image _imageForRotating;
    [SerializeField] private float _duration = 2f; // Rotation duration
    [SerializeField] private Vector3 _rotationAngle = new Vector3(0, 0, 360); 
    private Tween _rotationTween; // Reference to the animation for managing it

    private void OnEnable()
    {
        StartRotating(); // Start animation when the component is enabled
    }

    private void OnDisable()
    {
        StopRotating(); // Stop animation when the component is disabled
    }

    private void StartRotating()
    {
        if (_imageForRotating == null) return;

        // Start the animation and save its reference
        _rotationTween = _imageForRotating.rectTransform
            .DORotate(_rotationAngle, _duration, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart);
    }

    private void StopRotating()
    {
        if (_rotationTween != null && _rotationTween.IsActive())
        {
            _rotationTween.Kill(); // Stop the animation
            _rotationTween = null; // Clear the reference
        }
    }
}

