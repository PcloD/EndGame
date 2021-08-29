using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

/// <summary>
/// base class for anything entity related in world. Players & NPCs
/// </summary>
public class EntityNetworkBehaviour : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnHealthChanged))]
    public int Health;

    public EntityHpBar entityHpBar;

    private bool entityReady = false;

    void OnHealthChanged(int oldVal, int newVal)
    {
        Debug.Log($"SYNCVAR TRIGGER Old {oldVal} New {newVal}");
        Debug.Log(entityHpBar == null);
        entityHpBar?.SetHp(newVal / 100f);

        if (hasAuthority)
        {
            LocalTookDamage(oldVal - newVal);
        }
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        Debug.Log("On Start Client");
        entityReady = true;
    }

    [Server]
    public void ServerDealDamage(int damage, EntityNetworkBehaviour entityThatDidDamage)
    {
        Health -= damage;
        
        // tell the player who did dmg to show a dmg number
        TargetTookDamage(entityThatDidDamage.connectionToClient, damage);
    }

    private void LocalTookDamage(int damage)
    {
        if (!entityReady) return;
       Debug.Log($"I just took {damage} damage"); 
       CombatTextManager.DamageType type = CombatTextManager.DamageType.NORMAL;
       // TODO: implement damage types
       int test = Random.Range(0, 10);
       if (test < 3)
           type = CombatTextManager.DamageType.HEAL;
       else if (test < 6)
           type = CombatTextManager.DamageType.CRIT;
        
       CombatTextManager.Instance?.CreatePopup(entityHpBar.transform.position + Vector3.up, damage, type);
    }

    [TargetRpc]
    private void TargetTookDamage(NetworkConnection target, int damage)
    {
        CombatTextManager.DamageType type = CombatTextManager.DamageType.NORMAL;
        // TODO: implement damage types
        int test = Random.Range(0, 10);
        if (test < 3)
            type = CombatTextManager.DamageType.HEAL;
        else if (test < 6)
            type = CombatTextManager.DamageType.CRIT;
        
        Debug.Log($"I just dealt {damage} damage to {gameObject.name}");
        CombatTextManager.Instance.CreatePopup(entityHpBar.transform.position + Vector3.up, damage, type);
    }
    


    [ContextMenu("Lower HP")]
    private void Test_LowerHP()
    {
        Health -= 10;
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        Health = 100;
    }
}
