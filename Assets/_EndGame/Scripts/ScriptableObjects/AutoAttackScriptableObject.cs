using System;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
[CreateAssetMenu(fileName = "Data", menuName = "EndGame/LightAbility", order = 1)]
public class AutoAttackScriptableObject : AbilityScriptableObject
{
   [FormerlySerializedAs("LightAttackComboReset")] [Tooltip("In Seconds")]
   public float AutoAttackComboReset = 1f;
}