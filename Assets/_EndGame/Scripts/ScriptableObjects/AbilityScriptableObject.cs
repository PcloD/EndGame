using System;
using Sirenix.OdinInspector;
using UnityEngine;

public enum AbilityType
{
    Instant = 0,
    Direct = 1,
    SkillShot = 2,
    GroundTarget = 3
}

public enum AnimationType
{
    Auto0,
    Auto1
}

[Serializable]
public class AbilityScriptableObject : ScriptableObject
{
    [EnumToggleButtons]
    [Title("Animation Settings", TitleAlignment = TitleAlignments.Centered)]
    [GUIColor(1f,1f,0f)]
    [HideLabel]
    public AnimationType AnimationType;
    
    [EnumToggleButtons]
    [HideLabel]
    [Title("Ability Settings", TitleAlignment = TitleAlignments.Centered)]
    [GUIColor(0f,1f,1f)]
    public AbilityType AbilityType;
    
    //public DebuffScriptableObject[] Debuffs;
    
    [HorizontalGroup("Attack Duration")]
    [Tooltip("Time the attack will play for. After Casting if exists")]
    public float AttackDuration;
    
    [HorizontalGroup("Attack Duration")]
    [Tooltip("Time during the Attack when the damage will be dealt")]
    public float AttackTime;

    [InlineEditor(InlineEditorModes.FullEditor), PropertyOrder(10000)]
    public Ability AbilityPrefab;


    public string GetAnimationString()
    {
        switch (AnimationType)
        {
            case AnimationType.Auto0:
                return "Auto0";
            case AnimationType.Auto1:
                return "Auto1";
            default:
                return "";
        }
    }
}




