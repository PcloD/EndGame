using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Utils;
using FirstGearGames.Mirrors.Assets.FlexNetworkAnimators;
using Mirror;
using Pathfinding;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkPlayerBehaviour : EntityNetworkBehaviour
{
    public float MoveSpeed = 1f;
    public Transform IkAimTransform;

    private Animator animator;
    private FlexNetworkAnimator fna;
    [ReadOnly]public AIPath aStar;
    private EntityAbilityHandler entityAbilityHandler;

    private new Transform transform;
    
    private Vector3 velocityRelative;
    private Vector3 velocity;

    
    #region Initilization

    public override void OnStartClient()
    {
        base.OnStartClient();
        NetworkFirstInitilize();

        if (!hasAuthority) return;
        CameraManager.TrackedTransform = transform;
        CameraManager.TrackedIKTransform = IkAimTransform;
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        NetworkFirstInitilize();
    }

    void NetworkFirstInitilize()
    {
        if (!isServer)
        {
            Destroy(GetComponent<AIPath>());
            Destroy(GetComponent<RaycastModifier>());
        }
        
        transform = GetComponent<Transform>();
        animator = GetComponent<Animator>();
        fna = GetComponent<FlexNetworkAnimator>();   
        entityAbilityHandler = GetComponent<EntityAbilityHandler>();
        
        if (isServer)
        {
            aStar = GetComponent<AIPath>();
        }
    }

    #endregion

    void Update()
    {
        if (hasAuthority)
        {
            ClientMove();
            IkAimTransform.position = CameraManager.RayMouseHit.point;
        }

        if (isServer) OnServerUpdate();
    }

    void OnServerUpdate()
    {
        velocityRelative = transform.InverseTransformPoint(aStar.velocity + transform.position);
        velocity = aStar.velocity;
        SetAnimationValues();
        HandleRotation();
    }

    [Server]
    public bool IsMoving()
    {
        return aStar.hasPath && !aStar.isStopped;
    }

    [Client]
    void ClientMove()
    {
        // left or right click
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            if (MouseCursorManager.Instance.CurrentCursorType == MouseCursorManager.CursorType.Default)
            {
                MouseCursorManager.Instance.DoClickParticle();
                CmdRequestMove(CameraManager.RayMouseHit.point);
            }

            if (MouseCursorManager.Instance.CurrentCursorType == MouseCursorManager.CursorType.Attack)
            {
                CmdRequestAttack(MouseCursorManager.Instance.EnemyEntity);
            }
           
        }
    }

    [Server]
    void SetAnimationValues()
    {
        animator.SetFloat("ForwardSpeed", velocityRelative.z);
    }

    [Server]
    void HandleRotation()
    {
        // ability rotation
        if (entityAbilityHandler.currentAbility != null && entityAbilityHandler.entityTracker.TrackedEnemyTransform)
        {
            var lookDir = entityAbilityHandler.entityTracker.TrackedEnemyTransform.position - transform.position;
            lookDir.y = 0f;
            var abilityRotation = Quaternion.LookRotation(lookDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, abilityRotation, Time.fixedDeltaTime * 5f);
        }
        
        // movement rotation
        if (velocity.sqrMagnitude < 0.1f) return;
        var direction = velocity;
        direction.y = 0f;
        var lookRotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.fixedDeltaTime * 5f);
    }
    
    

    #region Server Commands

    [Command]
    private void CmdRequestMove(Vector3 mousePosition)
    {
        entityAbilityHandler.entityTracker.TrackedEnemyTransform = null;
        
        aStar.isStopped = false;
        
        aStar.destination = mousePosition;
        aStar.SearchPath();
    }

    [Command]
    private void CmdRequestAttack(GameObject entity)
    {
        entityAbilityHandler.entityTracker.TrackedEnemyTransform = entity.transform;
    }
    
    

    #endregion
}