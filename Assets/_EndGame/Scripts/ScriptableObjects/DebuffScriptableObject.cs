using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DebuffType
{
    Slow = 1,
    Stun = 2,
    DamageOverTime = 3
}
[Serializable]
[CreateAssetMenu(fileName = "Data", menuName = "EndGame/Debuff", order = 1)]
public class DebuffScriptableObject : ScriptableObject
{
    public DebuffType DebuffType;
    public float Duration;
}

public struct DebuffNetworkData
{
    public DebuffType DebuffType;
    public double NetworkExpirationTime;
}
