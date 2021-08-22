using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityHpBar : MonoBehaviour
{
    private float lastActivateTime;
    private float deactiveTime => lastActivateTime - Time.time + 3f;
    private bool isActive;

    [SerializeField] private Renderer ren;
    private MaterialPropertyBlock propertyBlock;

    private void Awake()
    {
        lastActivateTime = Time.time;
        propertyBlock = new MaterialPropertyBlock();
        ren.GetPropertyBlock(propertyBlock);

        propertyBlock.SetFloat("_fillRate", 1f);
        ren.SetPropertyBlock(propertyBlock);

        ren.enabled = false;
    }

    void Update()
    {
        if (!isActive) return;
        transform.LookAt(CameraManager.Instance.CameraTransform);
        if (deactiveTime < 0)
        {
            isActive = false;
            ren.enabled = false;
        }
    }

    public void SetHp(float hpValue)
    {
        ren.enabled = true;
        isActive = true;
        lastActivateTime = Time.time;
        propertyBlock.SetFloat("_fillRate", hpValue);
        ren.SetPropertyBlock(propertyBlock);
    }
}