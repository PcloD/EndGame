using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameArmoryManager : MonoBehaviour
{
    public static Dictionary<int,WeaponScriptableObject> WeaponScriptableObjects = new Dictionary<int, WeaponScriptableObject>();

    private void Awake()
    {
        var weaponSOs = Resources.LoadAll("GameData/Weapons/", typeof(WeaponScriptableObject)).Cast<WeaponScriptableObject>();
        
        Debug.Log(weaponSOs.Count());
        
        int count = 0;
        foreach (var abilitySO in weaponSOs)
        {
            WeaponScriptableObjects.Add(count, abilitySO);
            count++;
        }
    }

    public static int FindWeaponScriptableObjectIndex(WeaponScriptableObject weaponSo)
    {
        for (int i = 0; i < WeaponScriptableObjects.Count; i++)
        {
            if (WeaponScriptableObjects[i] == weaponSo) return i;
        }
        return -1;
    }
}
