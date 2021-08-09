using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundProjectile : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    
    private float destroyTime = -1f;
    public Vector3 targetPosition;
    
     private float catchupDistance = 0f;

     private Vector3 targetDelta => targetPosition - transform.position;
     
     void Update()
     {
         if (destroyTime < 0) return;
         
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
         
         if(Time.time > destroyTime || Vector3.Distance(transform.position, targetPosition) < 0.25f)
             Destroy(gameObject);
     }

     public void Initilize(float duration, float destroyTime, Vector3 targetPosition)
     {
         catchupDistance = (duration * moveSpeed);
         this.destroyTime = Time.time + destroyTime;
         this.targetPosition = targetPosition;

         transform.position = targetPosition + (Vector3.up * 10f);
     }
}
