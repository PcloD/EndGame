using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
[CreateAssetMenu(fileName = "Data", menuName = "EndGame/LightAbility", order = 1)]
public class AutoAttackScriptableObject : AbilityScriptableObject
{
   [Tooltip("In Seconds")]
   public float AutoAttackComboReset = 1f;
}