using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

public enum AutoHitStyle
{
    Melee = 0,
    Ranged = 1
}

[Serializable]
[CreateAssetMenu(fileName = "Data", menuName = "EndGame/Weapon", order = 1)]
public class WeaponScriptableObject : ScriptableObject
{
    public float WeaponDamage;
    public AutoHitStyle AutoHitStyle;
    public int AutoAttackRange;
    
    public AutoAttackScriptableObject[] AutoAttacks;
}
