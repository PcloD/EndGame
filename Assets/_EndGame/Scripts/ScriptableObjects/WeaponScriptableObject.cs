using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "Data", menuName = "EndGame/Weapon", order = 1)]
public class WeaponScriptableObject : ScriptableObject
{
    public float WeaponDamage;
    
    public LightAbilityScriptableObject[] LightAttacks;
    public HeavyAbilityScriptableObject HeavyAttack;

    public SpellAbilityScriptableObject Spell1;
    public SpellAbilityScriptableObject Spell2;
    public SpellAbilityScriptableObject Spell3;
}
