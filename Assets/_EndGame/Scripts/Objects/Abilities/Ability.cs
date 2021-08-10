using System;
using UnityEngine;

public class Ability : MonoBehaviour
{
    protected float DestroyTime = -1f;
    protected bool IsServer;
    protected Transform OwnerTransform;

    protected void Initilize(float destroyTime, Transform ownerTransform, bool isServer = false)
    {
        this.DestroyTime = destroyTime + Time.time;
        this.IsServer = isServer;
        this.OwnerTransform = ownerTransform;
    }

    public virtual void Update()
    {
        if (DestroyTime < 0) return;
        if (ShouldDestroy())
        {
            Destroy(gameObject);
        }
    }

    public virtual void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<PlayerNetworkBehaviour>();
        
        if (player != null && player.transform != OwnerTransform)
        {
            if (IsServer)
            {
                Debug.Log($"Server Do dmg");
            }
            Destroy(gameObject);
        }
    }

    public virtual void OnDestroy()
    {
        
    }

    protected virtual bool ShouldDestroy()
    {
        return Time.time > DestroyTime;
    }
    
    
    
    
}
