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
        if (!isActive)
        {
            return;
        }
        transform.LookAt(CameraManager.Instance.CameraTransform);
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
}