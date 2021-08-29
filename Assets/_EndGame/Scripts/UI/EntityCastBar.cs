using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class EntityCastBar : MonoBehaviour
{
    [SerializeField] private Renderer ren;
    private MaterialPropertyBlock propertyBlock;

    private NetworkPlayerBehaviour playerNb;
    private bool isInit;
    private void Awake()
    {
        propertyBlock = new MaterialPropertyBlock();
        ren.GetPropertyBlock(propertyBlock);

        propertyBlock.SetFloat("_fillRate", 1f);
        ren.SetPropertyBlock(propertyBlock);

        ren.enabled = false;
    }

    public void Init(NetworkPlayerBehaviour playerBehaviour)
    {
        playerNb = playerBehaviour;
        isInit = true;
    }

    private void Update()
    {
        if (!isInit) return;
        SetCasting();
    }
    void SetCasting()
    {
        if (playerNb.CurrentClientAbility != null && playerNb.CurrentClientAbility.IsCastable)
        {
            ren.enabled = true;
            
            propertyBlock.SetFloat("_fillRate", (float)playerNb.CurrentClientAbility.GetCurrentCastPercent);
            ren.SetPropertyBlock(propertyBlock);
        }
        else
        {
            ren.enabled = false;
        }
    }
}
