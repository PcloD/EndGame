using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class AbilityBarHUD : MonoBehaviour
{
    public List<UI_SocketAbility> AbilitySockets = new List<UI_SocketAbility>();

    public Transform LayoutParent;
    public UI_SocketAbility socketAbilityPrefab;
    
    private void Awake()
    {
        NetworkPlayerBehaviour.OnLocalClientReady += Init;
    }

    void Start()
    {
        NetworkPlayerBehaviour.Instance.equipmentInventory.SpellAbilitySockets.Callback += SpellAbilitySocketsOnCallback;
    }

    private void SpellAbilitySocketsOnCallback(SyncList<AbilityNetworkSocket>.Operation op, int itemindex, AbilityNetworkSocket olditem, AbilityNetworkSocket newitem)
    {
        AbilitySockets[itemindex].abilityNetworkSocket = newitem;
    }

    private void Init()
    {
        foreach (var abilityNetworkSlot in NetworkPlayerBehaviour.Instance.equipmentInventory.SpellAbilitySockets)
        {
            var abilitySlot = Instantiate(socketAbilityPrefab, LayoutParent);
            abilitySlot.abilityNetworkSocket = abilityNetworkSlot;
            AbilitySockets.Add(abilitySlot);
        }
    }
}
