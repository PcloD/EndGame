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

    void OnHealthChanged(int oldVal, int newVal)
    {
        Debug.Log($"SYNCVAR TRIGGER Old {oldVal} New {newVal}");
        Debug.Log(entityHpBar == null);
        entityHpBar?.SetHp(newVal / 100f);
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
