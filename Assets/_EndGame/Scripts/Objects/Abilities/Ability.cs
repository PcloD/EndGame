using System;
using UnityEngine;

public class Ability : MonoBehaviour
{
    protected float DestroyTime = -1f;
    protected bool IsServer;

    protected void Initilize(float destroyTime, bool isServer = false)
    {
        this.DestroyTime = destroyTime + Time.time;
        this.IsServer = isServer;
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
        
    }

    public virtual void OnDestroy()
    {
        
    }

    protected virtual bool ShouldDestroy()
    {
        return Time.time > DestroyTime;
    }
    
    
    
    
}
