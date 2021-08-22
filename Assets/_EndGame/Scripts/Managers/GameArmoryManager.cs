using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class GameArmoryManager : MonoBehaviour
{
    public static Dictionary<int,WeaponScriptableObject> WeaponScriptableObjects = new Dictionary<int, WeaponScriptableObject>();
    public static Dictionary<int,AbilityScriptableObject> AbilitySpellScriptableObjects = new Dictionary<int, AbilityScriptableObject>();

    private void Awake()
    {
        var weaponSOs = Resources.LoadAll("GameData/Weapons/", typeof(WeaponScriptableObject)).Cast<WeaponScriptableObject>();
        var abilitySpellSOs = Resources.LoadAll("GameData/Abilities/", typeof(AbilityScriptableObject)).Cast<AbilityScriptableObject>();
        
        Debug.Log(weaponSOs.Count());
        Debug.Log("Spell count : " +abilitySpellSOs.Count());
        
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
            AbilitySpellScriptableObjects.Add(abilitySo.AbilityId, abilitySo);
            abilitySpellCount++;
        }
    }

    [ContextMenu("Set Spell Ids")]
    public void SetSpellIds()
    {
        AssetDatabase.Refresh();
        var abilitySpellSOs = Resources.LoadAll("GameData/Abilities/", typeof(AbilityScriptableObject)).Cast<AbilityScriptableObject>();
        var idCount = 1;
        foreach (var spellAbilityScriptableObject in abilitySpellSOs)
        {
            spellAbilityScriptableObject.AbilityId = idCount;
            idCount++;
            EditorUtility.SetDirty(spellAbilityScriptableObject);
            AssetDatabase.SaveAssets();
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
