using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AbilityDirectProjectile : Ability
{
    [SerializeField] private float moveSpeed = 5f;
    
    public Vector3 targetPosition;
    public Vector3 Direction => (targetPosition - transform.position).normalized;

     public override void Update()
     {
         base.Update();
         
         float moveValue = moveSpeed * Time.deltaTime;
         
         
         // handle move
         transform.position += Direction * moveValue;
         
     }

     public void Initilize(bool isServer, float destroyTime, Vector3 targetPos, Transform ownerTransform)
     {
         Initilize(destroyTime, ownerTransform, isServer);
         targetPosition = targetPos;
     }
}
