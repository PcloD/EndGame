using System.Collections;
using System.Collections.Generic;
using FirstGearGames.Mirrors.Assets.FlexNetworkAnimators;
using Mirror;
using Pathfinding;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkPlayerBehaviour : NetworkBehaviour
{
    public float MoveSpeed = 1f;
    public Transform IkAimTransform;

    private Animator animator;
    private FlexNetworkAnimator fna;
    private IAstarAI aStar;

    private new Transform transform;
    private Vector3 movePosition;
    private Vector3 MoveDelta => (movePosition - transform.position).normalized;

    
    #region Initilization

    public override void OnStartClient()
    {
        base.OnStartClient();
        NetworkFirstInitilize();

        if (!hasAuthority) return;
        CameraManager.TrackedTransform = transform.Find("Mesh");
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

        if (isServer) aStar = GetComponent<AIPath>();

        transform = GetComponent<Transform>();
        animator = GetComponent<Animator>();
        fna = GetComponent<FlexNetworkAnimator>();
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
        SetAnimationValues();
        if (MoveDelta.sqrMagnitude > 0.1f)
        {
            
        }
    }

    [Client]
    void ClientMove()
    {
        if (Input.GetMouseButtonDown(0))
        {
            MouseCursorManager.Instance.ClickMoveParticle.transform.position = CameraManager.RayMouseHit.point + (Vector3.up * 0.01f);
            MouseCursorManager.Instance.ClickMoveParticle.Play();
          
            CmdRequestMove(CameraManager.RayMouseHit.point);
        }
    }

    [Server]
    void SetAnimationValues()
    {
        animator.SetFloat("ForwardSpeed", transform.InverseTransformPoint(aStar.velocity + transform.position).z);
    }

    [Server]
    void HandleRotation()
    {
        if (MoveDelta.sqrMagnitude < 0.1f) return;

        var direction = MoveDelta;
        direction.y = 0f;
        var lookRotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.fixedDeltaTime * 5f);
    }

    #region Server Commands

    [Command]
    private void CmdRequestMove(Vector3 mousePosition)
    {
        aStar.destination = mousePosition;
        aStar.SearchPath();
    }

    #endregion
}