using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Utils;
using UnityEditor;
using UnityEngine;

public class AbilityTarget : Ability
{
    [SerializeField] private float moveSpeed = 5f;

    [ReadOnly] public GameObject target;
    private Vector3 Direction => (target.transform.position - transform.position).normalized;

     public override void Update()
     {
         base.Update();
         
         var moveValue = moveSpeed * Time.deltaTime;
         // handle move
         transform.position += Direction * moveValue;
     }

     public override void OnTriggerEnter(Collider other)
     {
         base.OnTriggerEnter(other);
         if (other.gameObject != target) return;

         if (IsServer) other.GetComponent<EntityNetworkBehaviour>().Health -= 10;
         
         Destroy(gameObject);

     }

     public void Initilize(bool isServer, float destroyTime, GameObject target, EntityNetworkBehaviour ownerEntity)
     {
         Initilize(destroyTime, ownerEntity, isServer);
         this.target = target;
     }
}
