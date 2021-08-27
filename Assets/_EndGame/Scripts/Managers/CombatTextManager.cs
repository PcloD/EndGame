using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CombatTextManager : MonoBehaviour
{
    public enum DamageType
    {
        NORMAL,
        CRIT,
        HEAL
    }

    public static CombatTextManager Instance;
    
    public TMP_Style normalStyle;
    public TMP_Style critStyle;
    public TMP_Style healStyle;

    [SerializeField] private CombatTextPopup combatTextPopupPrefab;
    public Vector3 DestinationOffset;
    public Vector3 DestinationAltOffset;
    public float VisibleDuration = 1f;
    private void Awake()
    {
        Instance = this;
    }

    public void CreatePopup(Vector3 position, int dmgAmount, DamageType type)
    {
        CombatTextPopup combatTextPopup =
            Instantiate(combatTextPopupPrefab, position, Quaternion.identity);
        combatTextPopup.transform.LookAt((position - Vector3.forward) - CameraManager.Instance.CameraOffset);
        switch (type)
        {
            case DamageType.NORMAL:
                combatTextPopup.Display(dmgAmount, normalStyle, DestinationOffset);
                break;
            case DamageType.CRIT:
                combatTextPopup.Display(dmgAmount, critStyle, DestinationOffset);
                break;
            case DamageType.HEAL:
                combatTextPopup.Display(dmgAmount, healStyle, DestinationAltOffset);
                break;
        }
    }
}
