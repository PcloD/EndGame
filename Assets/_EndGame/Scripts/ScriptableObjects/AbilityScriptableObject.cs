using System;
using UnityEngine;

public enum AbilityType
{
    Instant = 0,
    Direct = 1,
    SkillShot = 2,
    GroundTarget = 3
}

[Serializable]
public class AbilityScriptableObject : ScriptableObject
{
    public string AnimationName;
    public AbilityType AbilityType;
    public DebuffScriptableObject[] Debuffs;
    
    [Tooltip("Time the attack will play for. After Casting if exists")]
    public float AttackDuration;
    
    [Tooltip("Time during the Attack when the damage will be dealt")]
    public float AttackTime;

    public Ability AbilityPrefab;
}




