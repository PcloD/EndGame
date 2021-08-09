using System;
using UnityEngine;

[Serializable]
public class AbilityData : ScriptableObject
{
    public string AnimationName;
    
    [Tooltip("Time the attack will play for. After Casting if exists")]
    public float AttackDuration;
    
    [Tooltip("Time during the Attack when the damage will be dealt")]
    public float AttackTime;
}




