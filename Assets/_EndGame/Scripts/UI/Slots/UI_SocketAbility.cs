using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_SocketAbility : UI_Socket
{
    private AbilityNetworkSocket _abilityNetworkSocket;

    public AbilityNetworkSocket abilityNetworkSocket
    {
        get { return _abilityNetworkSocket; }
        set
        {
            _abilityNetworkSocket = value;

            if (_abilityNetworkSocket.HasSpell)
            {
                // find the SO 
               var abilitySO = GameArmoryManager.AbilitySpellScriptableObjects[_abilityNetworkSocket.spellId] as SpellAbilityScriptableObject;
               IconImage.sprite = abilitySO.SpellSprite;
               IconImage.enabled = true;
            }
            else
            {
                IconImage.enabled = false;
            }
        }
    }

}
