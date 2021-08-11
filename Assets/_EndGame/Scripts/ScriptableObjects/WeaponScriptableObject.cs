using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "Data", menuName = "EndGame/WeaponData", order = 1)]
public class WeaponData : ScriptableObject
{
    public float WeaponDamage;
    
    public LightAbilityScriptableObject[] LightAttacks;
    public HeavyAbilityScriptableObject HeavyAttack;

    public SpellAbilityScriptableObject Spell1;
    public SpellAbilityScriptableObject Spell2;
    public SpellAbilityScriptableObject Spell3;
}
