using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class MinimapController : MonoBehaviour
{
    [SerializeField] Transform minimapCamera;
    [SerializeField] Transform targetTransform;

    public Vector3 CameraOffset;

    public void SetTarget(Transform target)
    {
        targetTransform = target;
    }

    void LateUpdate() {
        if(targetTransform == null)
            return;

        minimapCamera.position = targetTransform.position + CameraOffset;
    }
}
