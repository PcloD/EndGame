using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AbilityProjectile : Ability
{
    [SerializeField] private float moveSpeed = 5f;
    
    public Vector3 Direction;
    
     private float catchupDistance = 0f;

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
         transform.position += Direction * (moveValue + catchupValue);
         
     }

     public void Initilize(float duration,bool isServer, float destroyTime, Vector3 direction, Transform ownerTransform)
     {
         Initilize(destroyTime, ownerTransform, isServer);
         
         catchupDistance = (duration * moveSpeed);
         Direction = direction;
     }
}
