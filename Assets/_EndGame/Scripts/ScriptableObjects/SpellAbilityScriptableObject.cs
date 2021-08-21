using System;
using Sirenix.OdinInspector;
using UnityEngine;


[Serializable]
[CreateAssetMenu(fileName = "Data", menuName = "EndGame/SpellAbility", order = 1)]
public class SpellAbilityScriptableObject : AbilityScriptableObject
{
    public string AbilityName;

    [PreviewField(ObjectFieldAlignment.Center)]
    public Sprite SpellSprite;

}