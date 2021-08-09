using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    
    private float destroyTime = -1f;
    public Vector3 direction;
    
     private float catchupDistance = 0f;

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
         transform.position += direction * (moveValue + catchupValue);
         
         if(Time.time > destroyTime)
             Destroy(gameObject);
     }

     public void Initilize(float duration, float destroyTime, Vector3 direction)
     {
         catchupDistance = (duration * moveSpeed);
         this.destroyTime = Time.time + destroyTime;
         this.direction = direction;
     }
}
