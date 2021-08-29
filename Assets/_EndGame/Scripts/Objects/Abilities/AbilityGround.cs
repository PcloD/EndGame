using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Utils;
using UnityEngine;

public class AbilityGround : Ability
{
    [ReadOnly] public Vector3 GroundPosition;
    [ReadOnly] public float MoveSpeed;

     private Vector3 Direction => (GroundPosition - transform.position).normalized;

     public override void Update()
     {
         base.Update();
         
         float moveValue = MoveSpeed * Time.deltaTime;
       
         // handle move
         transform.position += Direction * moveValue;
         
         if(Vector3.Distance(transform.position, GroundPosition) < 0.1f) Destroy(gameObject);
     }

     public void Initilize(bool isServer, float destroyTime, Vector3 groundPosition, EntityNetworkBehaviour ownerEntity, float moveSpeed)
     {
         Initilize(destroyTime, ownerEntity,  isServer);
         MoveSpeed = moveSpeed;
         GroundPosition = groundPosition;

         transform.position = groundPosition + (Vector3.up * 10f);
     }
     
     public override void OnTriggerEnter(Collider other)
     {
         base.OnTriggerEnter(other);
         // dont hit self
         if (other.gameObject == OwnerEntity.gameObject) return;

         if (IsServer)
         {
             Debug.Log("Server do Dmg");
             var entity = other.GetComponent<EntityNetworkBehaviour>();
             if (entity != null)
             {
                ServerDoDmg(entity);
             }
         }
         Destroy(gameObject);
     }

}
