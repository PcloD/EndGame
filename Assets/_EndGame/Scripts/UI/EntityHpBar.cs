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
        propertyBlock = new MaterialPropertyBlock();
        ren.GetPropertyBlock(propertyBlock);
    }

    private void Start()
    {
        lastActivateTime = Time.time;
        isActive = false;
        ren.enabled = false;

        propertyBlock.SetFloat("_fillRate", 1f);
        ren.SetPropertyBlock(propertyBlock);
    }

    void Update()
    {
        if (!isActive)
        {
            Debug.Log($"Return hp bar");
            ren.enabled = false;
            return;
        }
        transform.LookAt(CameraManager.Instance.CameraTransform);
        if (deactiveTime < 0)
        {
            Debug.Log($"deactivate hp bar");
            isActive = false;
            ren.enabled = false;
        }
    }

    public void SetHp(float hpValue)
    {
        Debug.Log("Set HP bar");
        ren.enabled = true;
        isActive = true;
        lastActivateTime = Time.time;
        propertyBlock.SetFloat("_fillRate", hpValue);
        ren.SetPropertyBlock(propertyBlock);
    }
}