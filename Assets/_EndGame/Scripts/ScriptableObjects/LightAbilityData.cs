using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "Data", menuName = "EndGame/LightAbility", order = 1)]
public class LightAbilityData : AbilityData
{
   [Tooltip("In Seconds")]
   public float LightAttackComboReset = 1f;
}