using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityGroundProjectile : Ability
{
    [SerializeField] private float moveSpeed = 5f;
    
    public Vector3 TargetPosition;
    
     private float catchupDistance = 0f;

     private Vector3 targetDelta => (TargetPosition - transform.position).normalized;

     public override void Update()
     {
         base.Update();
         
         float moveValue = moveSpeed * Time.deltaTime;
         float catchupValue = 0f;

         if (catchupDistance > 0f)
         {
             float step = catchupDistance * Time.deltaTime;
             catchupDistance -= step;
             catchupValue = step;

             if (catchupDistance < (moveValue * 0.1f))
             {
                 catchupValue += catchupDistance;
                 catchupDistance = 0f;
             }
         }
         
         // handle move
         transform.position += targetDelta * (moveValue + catchupValue);
     }

     protected override bool ShouldDestroy()
     {
         return Time.time > DestroyTime || Vector3.Distance(transform.position, TargetPosition) < 0.25f;
     }

     public void Initilize(float duration,bool isServer, float destroyTime, Vector3 targetPosition)
     {
         Initilize(destroyTime, isServer);
         
         catchupDistance = (duration * moveSpeed);
         TargetPosition = targetPosition;

         transform.position = targetPosition + (Vector3.up * 10f);
     }
}
