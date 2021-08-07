using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "Data", menuName = "EndGame/WeaponData", order = 1)]
public class WeaponData : ScriptableObject
{
    public float WeaponDamage;
    public float LightAttackComboReset = 1f;
    public AbilityData[] LightAttacks;
    public AbilityData HeavyAttack;

}
