using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "Data", menuName = "EndGame/SpellAbility", order = 1)]
public class SpellAbilityScriptableObject : AbilityScriptableObject
{
    public bool projectile = true;
    public bool groundTarget = true;
    public float CastTime = 1f;
    
}