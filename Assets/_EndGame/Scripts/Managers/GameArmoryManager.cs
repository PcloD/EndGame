using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameArmoryManager : MonoBehaviour
{
    public static Dictionary<int,WeaponScriptableObject> WeaponScriptableObjects = new Dictionary<int, WeaponScriptableObject>();
    public static Dictionary<int,SpellAbilityScriptableObject> AbilitySpellScriptableObjects = new Dictionary<int, SpellAbilityScriptableObject>();

    private void Awake()
    {
        var weaponSOs = Resources.LoadAll("GameData/Weapons/", typeof(WeaponScriptableObject)).Cast<WeaponScriptableObject>();
        var abilitySpellSOs = Resources.LoadAll("GameData/Abilities/", typeof(SpellAbilityScriptableObject)).Cast<SpellAbilityScriptableObject>();
        
        Debug.Log(weaponSOs.Count());
        
        int weaponCount = 0;
        foreach (var weaponSo in weaponSOs)
        {
            Debug.Log(weaponCount + " Weapon - " + weaponSo.name);
            WeaponScriptableObjects.Add(weaponCount, weaponSo);
            weaponCount++;
        }
        
        int abilitySpellCount = 0;
        foreach (var abilitySo in abilitySpellSOs)
        {
            Debug.Log(abilitySpellCount + " Ability - " + abilitySo.name);
            AbilitySpellScriptableObjects.Add(abilitySpellCount, abilitySo);
            abilitySpellCount++;
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
