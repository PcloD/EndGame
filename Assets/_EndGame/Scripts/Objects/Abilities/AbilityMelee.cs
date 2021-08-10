using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityMelee : Ability
{
    public override void OnDestroy()
    {
        base.OnDestroy();

        if (IsServer)
        {
            
        }
    }
    
    public void Initilize(float destroyTime, bool isServer = false)
    {
        DestroyTime = destroyTime + Time.time;
        IsServer = isServer;
    }
}
