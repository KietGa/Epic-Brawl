using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceObjectToCamera : MonoBehaviour
{
    private void Update()
    {
        try
        {
            transform.LookAt(Camera.main.transform);
        }
        catch
        {

        }
    }
}
