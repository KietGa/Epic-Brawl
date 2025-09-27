using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerView : MonoBehaviour
{
    [SerializeField] private float mouseSensitive = 200f;
    [SerializeField] private float maxXRotation = 75f;

    private float xRotation;
    private float yRotation;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        MouseRotate();
    }

    private void MouseRotate()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitive * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitive * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -maxXRotation, maxXRotation);
        yRotation += mouseX;

        transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0);
    }
}
