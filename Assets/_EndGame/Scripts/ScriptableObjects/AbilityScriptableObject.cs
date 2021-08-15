using System;
using UnityEngine;

public enum AbilityType
{
    
}
[Serializable]
public class AbilityScriptableObject : ScriptableObject
{
    public string AnimationName;
    public DebuffScriptableObject[] Debuffs;
    
    [Tooltip("Time the attack will play for. After Casting if exists")]
    public float AttackDuration;
    
    [Tooltip("Time during the Attack when the damage will be dealt")]
    public float AttackTime;

    public Ability AbilityPrefab;
}




