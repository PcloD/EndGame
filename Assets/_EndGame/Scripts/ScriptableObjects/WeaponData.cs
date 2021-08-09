using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "Data", menuName = "EndGame/WeaponData", order = 1)]
public class WeaponData : ScriptableObject
{
    public float WeaponDamage;
    
    public LightAbilityData[] LightAttacks;
    public HeavyAbilityData HeavyAttack;

    public SpellAbilityData Spell1;
    public SpellAbilityData Spell2;
    public SpellAbilityData Spell3;
}
