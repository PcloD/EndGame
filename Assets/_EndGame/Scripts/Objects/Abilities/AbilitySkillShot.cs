using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Utils;
using UnityEngine;

public class AbilitySkillShot : Ability
{
    [ReadOnly] public Vector3 Direction;
    [ReadOnly] public float MoveSpeed;

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        var moveValue = MoveSpeed * Time.deltaTime;
        transform.position += Direction * moveValue;
    }

    public void Initilize(bool isServer, float destroyTime, Vector3 direction, EntityNetworkBehaviour ownerEntity, float moveSpeed)
    {
        Initilize(destroyTime,ownerEntity,isServer);
        Direction = direction;
        MoveSpeed = moveSpeed;
    }

    public override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        // dont hit self
        if (other.gameObject == OwnerEntity.gameObject) return;

        if (IsServer)
        {
            Debug.Log("Server do Dmg");
        }
        
        Destroy(gameObject);
    }
}
