using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Utils;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private Vector3 PositionOffset;
    [SerializeField] private Vector3 RotationOffset;

    [ReadOnly] public WeaponScriptableObject WeaponScriptableObject;

    public void Initilize(WeaponScriptableObject weaponScriptableObject)
    {
        transform.localPosition = PositionOffset;
        transform.localRotation = Quaternion.Euler(RotationOffset);
        this.WeaponScriptableObject = weaponScriptableObject;
    }
    
}
