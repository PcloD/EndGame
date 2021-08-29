using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EntityHpBar : MonoBehaviour
{
    private float lastActivateTime;
    private float deactiveTime => lastActivateTime - Time.time + 3f;
    private bool isActive;

    [SerializeField] private Renderer ren;
    private MaterialPropertyBlock propertyBlock;
    
    // Combat Text
    [SerializeField] private TMP_Text combatTxt;
    private float combatTxtDuration = 0;
    private float combatTxtEnd = 0;
    
    private float hpValue;

    private void Awake()
    {
        propertyBlock = new MaterialPropertyBlock();
        ren.GetPropertyBlock(propertyBlock);
    }

    private void Start()
    {
        lastActivateTime = Time.time;

        propertyBlock.SetFloat("_fillRate", hpValue);
        ren.SetPropertyBlock(propertyBlock);
    }

    void Update()
    {
        if (CameraManager.Instance == null) return;
        if (!isActive)
        {
            return;
        }
        
        if (deactiveTime < 0)
        {
            Debug.Log("Disable hp bar");
            ren.enabled = false;
            isActive = false;
            return;
        }
    }

    public void SetHp(float hp)
    {
        hpValue = hp;
        Debug.Log($"setting hp");
        ren.enabled = true;
        isActive = true;
        lastActivateTime = Time.time;
        propertyBlock.SetFloat("_fillRate", hpValue);
        ren.SetPropertyBlock(propertyBlock);
    }

    public IEnumerator DisplayDMG(int value)
    {
        combatTxt.text = value.ToString();
        combatTxt.enabled = true;
        combatTxtEnd = Time.timeSinceLevelLoad + combatTxtDuration;

        while (combatTxtEnd > Time.timeSinceLevelLoad)
        {
            yield return null;
        }

        combatTxt.enabled = false;
    }
}