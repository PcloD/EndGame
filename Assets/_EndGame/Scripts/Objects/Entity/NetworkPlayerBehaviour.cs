using System;
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
    public static NetworkPlayerBehaviour Instance;
    public float MoveSpeed = 1f;
    public Transform IkAimTransform;

    [ReadOnly] public Animator animator;
    [ReadOnly] public FlexNetworkAnimator fna;
    [ReadOnly] public AIPath aStar;
    [ReadOnly] public EntityAbilityHandler entityAbilityHandler;
    [ReadOnly] public EquipmentInventoryNB equipmentInventory;
    [ReadOnly] public EntityCastBar entityCastBar;

    [SyncVar (hook = "ClientAbilityChanged")] public CurrentAbility CurrentClientAbility;

    private new Transform transform;
    public GameObject PlayerCanvasPrefab;

    private Vector3 velocityRelative;
    private Vector3 velocity;

    public static Action OnLocalClientReady;
    #region Initilization

    public override void OnStartClient ()
    {
        base.OnStartClient ();
        NetworkFirstInitilize ();

        if (!hasAuthority) return;
        Instance = this;
        Instantiate (PlayerCanvasPrefab);
        equipmentInventory = GetComponent<EquipmentInventoryNB> ();
        CameraManager.TrackedTransform = transform;
        CameraManager.TrackedIKTransform = IkAimTransform;
        CameraManager.Instance.SetMinimapTarget(transform);
        Invoke ("DelayedReady", 3f);

    }

    void ClientAbilityChanged (CurrentAbility oldAbility, CurrentAbility newAbility)
    {
        Debug.Log ($"Client ability change {oldAbility != null} | {newAbility != null}");
        if (oldAbility != null && oldAbility.abilityId > 0)
        {
            Debug.Log ($"Old ability - {oldAbility.AbilitySO.name}");
        }

        if (newAbility != null && newAbility.abilityId > 0)
        {
            Debug.Log ($"new ability - {newAbility.AbilitySO.name}");
        }
    }

    /* todo figure out how to determine after all initial syncvars have fired client side
    todo cont - prob some kind of async function that waits for all the synclists to be populated / maybe server side send rpc
    */
    private void DelayedReady ()
    {
        OnLocalClientReady?.Invoke ();
        Debug.Log ("On Start client");
    }

    public override void OnStartServer ()
    {
        base.OnStartServer ();
        NetworkFirstInitilize ();
    }

    void NetworkFirstInitilize ()
    {
        if (!isServer)
        {
            Destroy (GetComponent<AIPath> ());
            Destroy (GetComponent<RaycastModifier> ());
        }

        transform = GetComponent<Transform> ();
        animator = GetComponent<Animator> ();
        fna = GetComponent<FlexNetworkAnimator> ();
        entityAbilityHandler = GetComponent<EntityAbilityHandler> ();
        entityCastBar = GetComponentInChildren<EntityCastBar> ();
        entityCastBar.Init (this);
        if (isServer)
        {
            aStar = GetComponent<AIPath> ();
        }
    }

    #endregion

    void Update ()
    {
        if (hasAuthority)
        {
            ClientMove ();
            IkAimTransform.position = CameraManager.RayMouseHit.point;
            if (CurrentClientAbility != null && CurrentClientAbility.abilityId > 0)
            {
                if (CurrentClientAbility.IsCastable) Debug.Log ($"Current Cast Time - {CurrentClientAbility.GetCurrentCastPercent}");
            }
        }

        if (isServer) OnServerUpdate ();
    }

    void OnServerUpdate ()
    {
        velocityRelative = transform.InverseTransformPoint (aStar.velocity + transform.position);
        velocity = aStar.velocity;
        SetAnimationValues ();
        HandleRotation ();
    }

    [Server]
    public bool IsMoving ()
    {
        return aStar.hasPath && !aStar.isStopped;
    }

    [Client]
    void ClientMove ()
    {
        // left or right click
        if (Input.GetMouseButtonDown (0) || Input.GetMouseButtonDown (1))
        {
            if (MouseCursorManager.Instance.CurrentCursorType == MouseCursorManager.CursorType.Default)
            {
                MouseCursorManager.Instance.DoClickParticle ();
                CmdRequestMove (CameraManager.RayMouseHit.point);
            }

            if (MouseCursorManager.Instance.CurrentCursorType == MouseCursorManager.CursorType.Attack)
            {
                CmdRequestAttack (MouseCursorManager.Instance.EnemyEntity);
            }
        }
    }

    [Server]
    void SetAnimationValues ()
    {
        animator.SetFloat ("ForwardSpeed", velocityRelative.z);
    }

    [Server]
    void HandleRotation ()
    {
        // ability rotation
        if (entityAbilityHandler.currentAbility != null && entityAbilityHandler.entityTracker.TrackedEnemyTransform)
        {
            var lookDir = entityAbilityHandler.entityTracker.TrackedEnemyTransform.position - transform.position;
            lookDir.y = 0f;
            var abilityRotation = Quaternion.LookRotation (lookDir);
            transform.rotation = Quaternion.Slerp (transform.rotation, abilityRotation, Time.fixedDeltaTime * 5f);
        }

        // movement rotation
        if (velocity.sqrMagnitude < 0.1f) return;
        var direction = velocity;
        direction.y = 0f;
        var lookRotation = Quaternion.LookRotation (direction);

        transform.rotation = Quaternion.Slerp (transform.rotation, lookRotation, Time.fixedDeltaTime * 5f);
    }

    #region Server Commands

    [Command]
    private void CmdRequestMove (Vector3 mousePosition)
    {
        entityAbilityHandler.entityTracker.TrackedEnemyTransform = null;

        aStar.isStopped = false;

        aStar.destination = mousePosition;
        aStar.SearchPath ();
    }

    [Command]
    private void CmdRequestAttack (GameObject entity)
    {
        entityAbilityHandler.entityTracker.TrackedEnemyTransform = entity.transform;
    }

    #endregion
}
