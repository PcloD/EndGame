using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private Vector3 PositionOffset;
    [SerializeField] private Vector3 RotationOffset;

    public WeaponScriptableObject WeaponScriptableObject;

    public void Initilize()
    {
        transform.localPosition = PositionOffset;
        transform.localRotation = Quaternion.Euler(RotationOffset);
    }
    
}
