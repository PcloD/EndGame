using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "Data", menuName = "EndGame/Ability", order = 1)]
public class AbilityData : ScriptableObject
{
    public string AnimationTriggerName;
    [Tooltip("Force the casting player will be moved")]
    public float AttackForce;
    [Tooltip("The speed at which the attack force will be negated")]
    public float AttackNegativeForce;
    [Tooltip("Force the hit opponent will be moved")]
    public float RecoilForce;
    [Tooltip("In Seconds")]
    public float CoolDown;
    [Tooltip("In Seconds")]
    public float AttackDuration;
    [Tooltip("In Seconds")]
    public float AttackTime;
}
