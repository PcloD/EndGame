using System;
using System.Collections.Generic;
using Assets.Scripts.Utils;
using FirstGearGames.Mirrors.Assets.FlexNetworkAnimators;
using FirstGearGames.Utilities.Maths;
using Mirror;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class PlayerNetworkBehaviour : NetworkBehaviour
{
    public PlayerMovementData MoveData = new PlayerMovementData();
    private InputData _storedInputs = new InputData();

    [ReadOnly] public float AttackVelocityNegativeForce = 3f;

    // using this to reset triggers
    [ReadOnly]public List<AnimationTrigger> AnimationTriggers = new List<AnimationTrigger>();

    #region PositionSyncing Vars

    private List<ClientPlayerMotorState> _clientMotorStates = new List<ClientPlayerMotorState>();
    private Queue<ClientPlayerMotorState> _receivedClientMotorStates = new Queue<ClientPlayerMotorState>();

    /// <summary>
    /// Last FixedFrame processed from client.
    /// </summary>
    private uint _lastClientStateReceived = 0;

    /// <summary>
    /// Most current motor state received from the server.
    /// </summary>
    private ServerPlayerMotorState? _receivedServerMotorState = null;

    /// <summary>
    /// Maximum number of entries that may be held within ReceivedClientMotorStates.
    /// </summary>
    private const int MAXIMUM_RECEIVED_CLIENT_MOTOR_STATES = 10;

    /// <summary>
    /// How many past states to send to the server.
    /// </summary>
    private const int PAST_STATES_TO_SEND = 3;

    #endregion

    private Animator animator;

    public WeaponData TestWeapon;

    private FlexNetworkAnimator _fna;
    private CharacterController _characterController = null;
    public AbilityHandler _abilityHandler;
    public Transform IkAimTransform;

    public double GetNetworkTime => NetworkTime.time;

    public override void OnStartServer()
    {
        base.OnStartServer();
        NetworkFirstInitialize();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        NetworkFirstInitialize();
        if (base.hasAuthority)
        {
            CameraManager.TrackedTransform = transform.Find("Mesh");
            CameraManager.TrackedIKTransform = IkAimTransform;
        }
    }

    private void OnEnable()
    {
        FixedUpdateManager.OnFixedUpdate += FixedUpdateManager_OnFixedUpdate;
    }

    private void OnDisable()
    {
        FixedUpdateManager.OnFixedUpdate -= FixedUpdateManager_OnFixedUpdate;
    }

    private void Update()
    {
        if(isServer)_abilityHandler.OnServerUpdate();
        
        if (base.hasAuthority)
        {
            TryRoll();
            TryBlock();
            TryLightAttack();
            
            // idk if i needa run this on server also
            foreach (var animationTrigger in AnimationTriggers)
            {
                if (Time.time > animationTrigger.TimeRegistered + 0.2f)
                {
                    animator.ResetTrigger(animationTrigger.Hash);
                    AnimationTriggers.Remove(animationTrigger);
                    return;
                }
            }
        }
    }

    /// <summary>
    /// Received when a simulated fixed update occurs.
    /// </summary>
    private void FixedUpdateManager_OnFixedUpdate()
    {
        if (base.hasAuthority)
        {
            ProcessReceivedServerMotorState();
            SendInputs();
        }

        if (base.isServer)
        {
            ProcessReceivedClientMotorState();
        }
    }

    void HandlePlayerRotation(Vector3 mousePosition)
    {
        MoveData.MouseDelta = mousePosition - transform.position;
        var angleFwdMouse = Vector3.Angle(transform.forward, MoveData.MouseDelta);
        var dotFwd = Vector3.Dot(MoveData.MouseDelta.normalized, MoveData.Velocity.normalized);

        var rot = MoveData.MouseDelta;
        rot.y = 0;
        
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(rot),
            Time.fixedDeltaTime * MoveData.BaseRotationSpeed);
      //  Debug.Log($"Velocity - {transform.TransformDirection(_characterController.velocity).normalized}");
        
        
      /*
        // calculate if we should be moving backwards
        if (Mathf.Abs(dotFwd) > 0.1f)
        {
            MoveData.BackMoving = dotFwd < 0;
        }
        
        Debug.Log(Vector3.Dot(_characterController.velocity.normalized,  MoveData.MouseDelta.normalized));

        // invert velocity if moving backwards
        MoveData.Velocity = MoveData.BackMoving ? MoveData.Velocity * -1f : MoveData.Velocity;

        // dont rotate if rolling 
        if (MoveData.IsRolling) return;

        // if not moving we cant use velocity. so use mouse delta instead
        var rotAim = transform.position +
                     (MoveData.Velocity.normalized.magnitude < 0.1f ? MoveData.MouseDelta : MoveData.Velocity.normalized);

        var rotVelocityDelta = rotAim - transform.position;

        rotVelocityDelta.y = 0f;
        MoveData.MouseDelta.y = 0f;

        var finalRotation = angleFwdMouse > 100
            ? Quaternion.LookRotation(MoveData.MouseDelta)
            : Quaternion.LookRotation(rotVelocityDelta);
        transform.rotation = Quaternion.Lerp(transform.rotation, finalRotation,
            Time.fixedDeltaTime * MoveData.BaseRotationSpeed);
           */
    }

    /// <summary>
    /// Set on Owning client & server. propigated with FlexNetworkAnimator
    /// </summary>
    void SetAnimationValues()
    {
        var speed = MoveData.BackMoving ? MoveData.Velocity.sqrMagnitude * -1f : MoveData.Velocity.sqrMagnitude;
        animator.SetFloat("Speed", speed);
        animator.SetBool("Idle", Mathf.Abs(speed) < 0.2f);
        
        
        animator.SetFloat("Vertical", MoveData.Velocity.z);
        animator.SetFloat("Horizontal", MoveData.Velocity.x);
    }

    /// <summary>
    /// Initializes this script for use. Should only be called once.
    /// </summary>
    private void NetworkFirstInitialize()
    {
        _characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        _fna = GetComponent<FlexNetworkAnimator>();
        if (!base.isServer && !base.hasAuthority) _characterController.enabled = false;
        if(isServer || hasAuthority) _abilityHandler = new AbilityHandler(animator, _fna, TestWeapon, this);
    }

    [Client]
    private void TryRoll()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            _storedInputs.Dodge = true;
        }
    }

    [Client]
    private void TryLightAttack()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _storedInputs.LightAttack = true;
        }
    }

    [Client]
    private void TryBlock()
    {
        if (Input.GetKey(KeyCode.R))
        {
            _storedInputs.Block = true;
        }
    }

    /// <summary>
    /// Processes the last received server motor state.
    /// </summary>
    [Client]
    private void ProcessReceivedServerMotorState()
    {
        if (_receivedServerMotorState == null) return;

        ServerPlayerMotorState serverState = _receivedServerMotorState.Value;
        FixedUpdateManager.AddTiming(serverState.TimingStepChange);
        _receivedServerMotorState = null;

        //Remove entries which have been handled by the server.
        int index = _clientMotorStates.FindIndex(x => x.FixedFrame == serverState.FixedFrame);
        if (index != -1) _clientMotorStates.RemoveRange(0, index + 1);

        //Snap motor to server values.
        transform.position = serverState.Position;
        // transform.rotation = serverState.Rotation;

        MoveData.DodgeVelocity = serverState.DodgeVelocity;
        MoveData.AttackVelocity = serverState.AttackVelocity;
        MoveData.RecoilVelocity = serverState.RecoilVelocity;

        Physics.SyncTransforms();

        foreach (ClientPlayerMotorState clientState in _clientMotorStates)
        {
            ProcessInputs(clientState);
            //Physics.Simulate(Time.fixedDeltaTime);
        }
    }

    /// <summary>
    /// Processes the last received client motor state.
    /// </summary>
    [Server]
    private void ProcessReceivedClientMotorState()
    {
        // host only mode
        if (base.isClient && base.hasAuthority) return;

        sbyte timingStepChange = 0;

        /* If there are no states then set timing change step
        * to a negative value, which will speed up the client
        * simulation. In result this will increase the chances
        * the client will send a packet which will arrive by every
        * fixed on the server. */
        if (_receivedClientMotorStates.Count == 0)
            timingStepChange = -1;
        /* Like subtracting a step, if there is more than one entry
        * then the client is sending too fast. Send a positive step
        * which will slow the clients send rate. */
        else if (_receivedClientMotorStates.Count > 1) timingStepChange = 1;

        //If there is input to process.
        if (_receivedClientMotorStates.Count > 0)
        {
            ClientPlayerMotorState state = _receivedClientMotorStates.Dequeue();
            //Process input of last received motor state.
            ProcessInputs(state);

            ServerPlayerMotorState responseState = new ServerPlayerMotorState
            {
                FixedFrame = state.FixedFrame,
                Position = transform.position,
                DodgeVelocity = MoveData.DodgeVelocity,
                AttackVelocity = MoveData.AttackVelocity,
                RecoilVelocity = MoveData.RecoilVelocity,
                TimingStepChange = timingStepChange
            };

            //Send results back to the owner.
            TargetServerStateUpdate(base.connectionToClient, responseState);
        }
        //If there is no input to process.
        else if (timingStepChange != 0)
        {
            //Send timing step change to owner.
            TargetChangeTimingStep(base.connectionToClient, timingStepChange);
        }
    }

    private void ProcessMovementActionCodes(ActionCodes actionCode)
    {
        switch (actionCode)
        {
            case ActionCodes.Block:
            {
                if (MoveData.CanBlockOrDodge && _abilityHandler.CurrentAbility == null && Time.time > _abilityHandler.AbilityClearedTimed + 0.25f)
                {
                    MoveData.MovementVelocityDamp -= 0.3f;
                    animator.SetBool("Block", MoveData.CanBlockOrDodge);
                }

                break;
            }
            case ActionCodes.Dodge:
            {
                if (MoveData.CanBlockOrDodge && _abilityHandler.CurrentAbility == null  && Time.time > _abilityHandler.AbilityClearedTimed + 0.25f)
                {
                    MoveData.DodgeVelocity = MoveData.MovementInput.normalized.magnitude < 0.1f ? transform.forward * (MoveData.DodgeForce * 1.2f) : MoveData.MovementInput.normalized * MoveData.DodgeForce;
                    _fna.SetTrigger("Dodge");
                    AnimationTriggers.Add(new AnimationTrigger("Dodge"));
                }
                break;
            }
            default:
                animator.SetBool("Block", false);
                break;
        }
    }

    /// <summary>
    /// Processes input from a state.
    /// </summary>
    /// <param name="motorState"></param>
    private void ProcessInputs(ClientPlayerMotorState motorState)
    {
        motorState.Horizontal = Floats.PreciseSign(motorState.Horizontal);
        motorState.Forward = Floats.PreciseSign(motorState.Forward);
        // todo fix this 
        IkAimTransform.transform.position = motorState.MousePosition + Vector3.up;

        MoveData.MovementInput = new Vector3(motorState.Horizontal, 0f, motorState.Forward);
        Vector3 moveDirection = new Vector3(motorState.Horizontal, 0f, motorState.Forward);

        // clamp diagonal speed
        moveDirection = moveDirection.magnitude > 1f ? moveDirection.normalized : moveDirection;

        //Add move direction.
        MoveData.MovementVelocityDamp = 1f;
        ProcessMovementActionCodes(motorState.ActionCode);
        _abilityHandler.HandleAbility(motorState.ActionCode);

        if (MoveData.BackMoving) MoveData.MovementVelocityDamp -= 0.25f;

        MoveData.MovementVelocityDamp = Mathf.Clamp(MoveData.MovementVelocityDamp, 0, 1);
        MoveData.MovementVelocityDamp = MoveData.BaseMoveSpeed * MoveData.MovementVelocityDamp;

        if (Mathf.Abs(MoveData.AttackVelocity.magnitude) > 0.1f)
        {
            MoveData.MovementVelocityDamp -= 0.9f;
            moveDirection = (moveDirection + MoveData.AttackVelocity) * (MoveData.MovementVelocityDamp * Time.deltaTime);
        }
        else
        {
            moveDirection = (moveDirection + MoveData.DodgeVelocity) * (MoveData.MovementVelocityDamp * Time.fixedDeltaTime);
        }

        // lower velocities 
        MoveData.AttackVelocity = Vector3.MoveTowards(MoveData.AttackVelocity, Vector3.zero,
            Time.fixedDeltaTime * AttackVelocityNegativeForce);
        MoveData.DodgeVelocity = Vector3.MoveTowards(MoveData.DodgeVelocity, Vector3.zero,
            Time.fixedDeltaTime * MoveData.NegativeDodgeForce);

        //Move character.
        _characterController.Move(moveDirection);

        Debug.Log(MoveData.MovementInput);
        MoveData.Velocity = transform.InverseTransformDirection(MoveData.MovementInput);//_characterController.velocity;
        MoveData.Velocity.x = Mathf.Clamp(transform.InverseTransformDirection(MoveData.MovementInput).x,-1,1);
        MoveData.Velocity.z = Mathf.Clamp(transform.InverseTransformDirection(MoveData.MovementInput).z,-1,1);

        HandlePlayerRotation(motorState.MousePosition);
        SetAnimationValues();
    }

    /// <summary>
    /// Sends inputs for the client.
    /// </summary>
    [Client]
    private void SendInputs()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float forward = Input.GetAxisRaw("Vertical");

        /* Action Codes. */
        ActionCodes ac = ActionCodes.None;

        if (_storedInputs.Block)
        {
            _storedInputs.Block = false;
            ac = ActionCodes.Block;
        }

        if (_storedInputs.Dodge)
        {
            _storedInputs.Dodge = false;
            ac = ActionCodes.Dodge;
        }

        if (_storedInputs.LightAttack)
        {
            _storedInputs.LightAttack = false;
            ac = ActionCodes.LightAttack;
        }

        ClientPlayerMotorState state = new ClientPlayerMotorState
        {
            FixedFrame = FixedUpdateManager.FixedFrame,
            Horizontal = horizontal,
            Forward = forward,
            ActionCode = ac,
            MousePosition = CameraManager.RayMouseHit.point
        };
        _clientMotorStates.Add(state);

        //Only send at most up to client motor states count.
        int targetArraySize = Mathf.Min(_clientMotorStates.Count, 1 + PAST_STATES_TO_SEND);
        //Resize array to accomodate 
        ClientPlayerMotorState[] statesToSend = new ClientPlayerMotorState[targetArraySize];
        /* Start at the end of cached inputs, and add to the end of inputs to send.
         * This will add the older inputs first. */
        for (int i = 0; i < targetArraySize; i++)
        {
            //Add from the end of states first.
            statesToSend[targetArraySize - 1 - i] = _clientMotorStates[_clientMotorStates.Count - 1 - i];
        }

        ProcessInputs(state);
        CmdSendInputs(statesToSend);
    }

    /// <summary>
    /// Send inputs from client to server.
    /// </summary>
    /// <param name="states"></param>
    [Command(channel = 1)]
    private void CmdSendInputs(ClientPlayerMotorState[] states)
    {
        //No states to process.
        if (states == null || states.Length == 0) return;
        //Only for client host.
        if (base.isClient && base.hasAuthority) return;

        /* Go through every new state and if the fixed frame
         * for that state is newer than the last received
         * fixed frame then add it to motor states. */
        for (int i = 0; i < states.Length; i++)
        {
            if (states[i].FixedFrame > _lastClientStateReceived)
            {
                _receivedClientMotorStates.Enqueue(states[i]);
                _lastClientStateReceived = states[i].FixedFrame;
            }
        }

        while (_receivedClientMotorStates.Count > MAXIMUM_RECEIVED_CLIENT_MOTOR_STATES)
            _receivedClientMotorStates.Dequeue();
    }

    /// <summary>
    /// Received on the owning client after the server processes ClientMotorState.
    /// </summary>
    /// <param name="conn"></param>
    /// <param name="state"></param>
    [TargetRpc(channel = 1)]
    private void TargetServerStateUpdate(NetworkConnection conn, ServerPlayerMotorState state)
    {
        //Exit if received state is older than most current.
        if (_receivedServerMotorState != null && state.FixedFrame < _receivedServerMotorState.Value.FixedFrame) return;

        _receivedServerMotorState = state;
    }

    /// <summary>
    /// Received on the owning client after server fails to process any inputs.
    /// </summary>
    /// <param name="state"></param>
    [TargetRpc(channel = 1)]
    private void TargetChangeTimingStep(NetworkConnection conn, sbyte steps)
    {
        FixedUpdateManager.AddTiming(steps);
    }
}