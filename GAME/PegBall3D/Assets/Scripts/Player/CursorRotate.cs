using System;
using UnityEngine;

public class CursorRotate : MonoBehaviour
{
    public float rotationAmount = 1.75f; // Max degrees to rotate

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

        float targetX = -y * rotationAmount; // invert Y
        float targetY = x * rotationAmount;

        Quaternion targetRotation = Quaternion.Euler(defaultRotation.x + targetX, defaultRotation.y + targetY, defaultRotation.z);

        // rotate camera
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, Time.deltaTime * _smoothSpeed);
    }
}
