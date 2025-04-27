using System;
using UnityEngine;

public class MouseRotateCamera : MonoBehaviour
{
    private const float RotationAmountUnfocused = 2.75f; // Max degrees to rotate unfocused
    private const float RotationAmountFocused = .55f; // Max degrees to rotate focused

    private float _rotationAmount;

    private float _smoothSpeed = 5f;

    Vector3 defaultRotation;
    
    private Vector3 rotationEuler;
    private Vector2 _cursorAmountFromCenter;

    private void Start()
    {
        // save original rotation
        defaultRotation = transform.localEulerAngles;
    }

    void Update()
    {
        Vector2 mousePos = Input.mousePosition;

        float x = (mousePos.x / Screen.width - 0.5f) * 2f;
        float y = (mousePos.y / Screen.height - 0.5f) * 2f;

        _rotationAmount = GameMaster.Instance.IsPlayerFocused ? RotationAmountFocused : RotationAmountUnfocused;

        float targetX = -y * _rotationAmount; // invert Y
        float targetY = x * _rotationAmount;

        Quaternion targetRotation = Quaternion.Euler(defaultRotation.x + targetX, defaultRotation.y + targetY, defaultRotation.z);

        // rotate camera
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, Time.deltaTime * _smoothSpeed);
    }
}
