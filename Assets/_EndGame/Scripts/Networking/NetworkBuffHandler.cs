using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

/// <summary>
/// class responsible for syncing Buffs/Debuffs across the network
/// </summary>
public class NetworkBuffHandler : NetworkBehaviour
{
    public bool IsSlowed;
    [Serializable]
    public struct DebuffData
    {
        public DebuffType DebuffType;
        public double ExpireTime;
    }
    
    public List<DebuffData> Debuffs = new List<DebuffData>();

    [Server]
    public void AddDebuff(DebuffScriptableObject data)
    {
        DebuffData debuffData = new DebuffData
        {
            DebuffType = data.DebuffType, ExpireTime = NetworkTime.time + data.Duration
        };

        Debuffs.Add(debuffData);
        RpcSendDebuff(debuffData);
    }

    [Client]
    public void AddDummyDebuff(DebuffScriptableObject data)
    {
        DebuffData debuffData = new DebuffData
        {
            DebuffType = data.DebuffType, ExpireTime = NetworkTime.time + 0.5f
        };
        Debuffs.Add(debuffData);
    }

    [ClientRpc]
    private void RpcSendDebuff(DebuffData debuffData)
    {
        Debug.Log("Client adding debuff");
        Debuffs.Add(debuffData);
    }

    void Update()
    {
        if(hasAuthority || isServer) ProcessDebuffs();
    }


    void ProcessDebuffs()
    {
        var isSlowed = false;
        
        foreach (var debuffData in Debuffs)
        {
            // check if debuff has expired
            if (debuffData.ExpireTime < NetworkTime.time)
            {
                Debuffs.Remove(debuffData);
                return;
            }
            if (debuffData.DebuffType == DebuffType.Slow) isSlowed = true;
        }
        IsSlowed = isSlowed;
    }
    
}
