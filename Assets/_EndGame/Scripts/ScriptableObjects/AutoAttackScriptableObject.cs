using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "Data", menuName = "EndGame/LightAbility", order = 1)]
public class AutoAttackScriptableObject : AbilityScriptableObject
{
   [Tooltip("In Seconds")]
   public float LightAttackComboReset = 1f;
}