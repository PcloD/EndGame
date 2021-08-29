using System;
using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    private void Update()
    {
        if (CameraManager.Instance == null) return;
        
        transform.LookAt(transform.position - Vector3.forward + CameraManager.Instance.CameraOffset);
    }
}