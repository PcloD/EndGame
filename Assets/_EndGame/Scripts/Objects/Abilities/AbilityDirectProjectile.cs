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
         
         
         // handle move
         transform.position += Direction * moveValue;
         
     }

     public void Initilize(float duration,bool isServer, float destroyTime, Vector3 direction, Transform ownerTransform)
     {
         Initilize(destroyTime, ownerTransform, isServer);
         
         catchupDistance = (duration * moveSpeed);
         Direction = direction;
     }
}
