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
}
