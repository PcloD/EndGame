using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "Data", menuName = "EndGame/SpellAbility(Not Implemented)", order = 1)]
public class SpellAbilityData : AbilityData
{
    public bool projectile = true;
    public bool groundTarget = true;
    public float CastTime = 1f;
    
}