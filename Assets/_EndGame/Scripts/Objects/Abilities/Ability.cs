using System;
using UnityEngine;

public class Ability : MonoBehaviour
{
    protected float DestroyTime = -1f;
    protected bool IsServer;
    protected EntityNetworkBehaviour OwnerEntity;

    protected void Initilize(float destroyTime, EntityNetworkBehaviour ownerEntity, bool isServer = false)
    {
        this.DestroyTime = destroyTime + Time.time;
        this.IsServer = isServer;
        this.OwnerEntity = ownerEntity;
    }

    public virtual void Update()
    {
        if (DestroyTime < 0) return;
        if (ShouldDestroy())
        {
            Destroy(gameObject);
        }
    }
    protected void ServerDoDmg(EntityNetworkBehaviour entityNb)
    {
        entityNb.ServerDealDamage(10, OwnerEntity);
    }

    public virtual void OnTriggerEnter(Collider other)
    {
       
    }

    public virtual void OnDestroy()
    {
        
    }

    protected virtual bool ShouldDestroy()
    {
        return Time.time > DestroyTime;
    }
    
    
    
    
}
