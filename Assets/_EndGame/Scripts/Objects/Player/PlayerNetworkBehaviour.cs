using System;
using System.Collections.Generic;
using Assets.Scripts.Utils;
using FirstGearGames.Mirrors.Assets.FlexNetworkAnimators;
using FirstGearGames.Utilities.Maths;
using Mirror;
using RootMotion.FinalIK;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class PlayerNetworkBehaviour : NetworkBehaviour
{
    public PlayerMovementData MoveData = new PlayerMovementData();
    public NetworkBuffHandler networkBuffHandler;
    public InputData InputData;
    private NetworkAbilityHandler networkAbilityHandler;
    private LookAtIK _lookatIk;

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

    private FlexNetworkAnimator _fna;
    private CharacterController _characterController = null;
    public Transform IkAimTransform;

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
        if (base.hasAuthority)
        {
            InputData.HandleInputs();
            
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
            networkAbilityHandler.SendAbility();
        }

        if (base.isServer)
        {
            ProcessReceivedClientMotorState();
        }
    }

    void HandlePlayerMovement(ClientPlayerMotorState motorState)
    {
        Vector3 moveDirection = new Vector3(motorState.Horizontal, 0f, motorState.Forward);

        // clamp diagonal speed
        moveDirection = moveDirection.magnitude > 1f ? moveDirection.normalized : moveDirection;

        //Add move direction.
        MoveData.MovementVelocityDamp = 1f;
        
        ProcessMovementActionCodes(motorState);

        if (MoveData.BackMoving) MoveData.MovementVelocityDamp -= 0.25f;
        if (motorState.ActiveAbility) MoveData.MovementVelocityDamp = 0.1f;
        if (motorState.IsSlowed) MoveData.MovementVelocityDamp -= 0.5f;

        MoveData.IsSlowed = motorState.IsSlowed;
        MoveData.MovementVelocityDamp = Mathf.Clamp(MoveData.MovementVelocityDamp, 0, 1);
        MoveData.MovementVelocityDamp = MoveData.BaseMoveSpeed * MoveData.MovementVelocityDamp;

        if (_characterController.isGrounded)
        {
            moveDirection.y = 0;
        }
        else
        {
            moveDirection.y -= 50f * Time.fixedDeltaTime;
        }
        
        moveDirection = (moveDirection + MoveData.DodgeVelocity) * (MoveData.MovementVelocityDamp * Time.fixedDeltaTime);
        
        // lower velocities 
        //MoveData.AttackVelocity = Vector3.MoveTowards(MoveData.AttackVelocity, Vector3.zero,
        //    Time.fixedDeltaTime * AttackVelocityNegativeForce);
        MoveData.DodgeVelocity = Vector3.MoveTowards(MoveData.DodgeVelocity, Vector3.zero,
            Time.fixedDeltaTime * MoveData.NegativeDodgeForce);

        //Move character.
        _characterController.Move(moveDirection); 
    }

    void HandlePlayerRotation(Vector3 mousePosition)
    {
        if (MoveData.IsRolling)
        {
            _lookatIk.solver.IKPositionWeight = 0f;
            var aimTarget = transform.position +  (MoveData.BackMoving ? -MoveData.DodgeVelocity : MoveData.DodgeVelocity);
            var direction = aimTarget - transform.position;
            direction.y = 0f;
            
            var lookRotation = Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.fixedDeltaTime * 5f);
            return;
        }

        if (networkAbilityHandler.IsAttacking)
        {
            Vector3 aimPosition = Vector3.zero;
            
            if (networkAbilityHandler.currentAbility != null)
            {
                var spellAbility = networkAbilityHandler.currentAbility.abilityScriptableObject as SpellAbilityScriptableObject;
                if (spellAbility != null)
                {
                    aimPosition = spellAbility.projectile
                        ? networkAbilityHandler.currentAbility.abilityTarget
                        : networkAbilityHandler.currentAbility.abilityTarget - transform.position;
                }
                else
                {
                    aimPosition = networkAbilityHandler.currentAbility.abilityTarget;

                }
            }
            else
            {
                aimPosition = MoveData.MouseDelta;
            }
           
            IkAimTransform.transform.position = aimPosition + transform.position + Vector3.up;

            if (aimPosition.sqrMagnitude < 0.1f) return;
            var lookRotation = Quaternion.LookRotation(aimPosition);

            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.fixedDeltaTime * 5f);
            return;
        }
        
        _lookatIk.solver.IKPositionWeight = 1f;
       
        
        // todo fix this 
        IkAimTransform.transform.position = Vector3.Lerp(IkAimTransform.position,mousePosition + Vector3.up, Time.fixedDeltaTime * 10f);
        
        var rot = MoveData.MouseDelta;
        if (rot.magnitude < 0.1f) return;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(rot), Time.fixedDeltaTime * MoveData.BaseRotationSpeed);
    }

    /// <summary>
    /// Set on Owning client & server. propigated with FlexNetworkAnimator
    /// </summary>
    void SetAnimationValues()
    {
        var normalizedVel = transform.InverseTransformDirection(MoveData.MovementInput).normalized;
        var currentVerticle = animator.GetFloat("Vertical");
        var currentHorrizonal = animator.GetFloat("Horizontal");
        
        animator.SetFloat("Speed", MoveData.RelativeVelocity.z); // don't use the normalized value here
        animator.SetFloat("Vertical", Mathf.Lerp(currentVerticle,normalizedVel.z, Time.fixedDeltaTime * 5f));
        animator.SetFloat("Horizontal",Mathf.Lerp(currentHorrizonal,normalizedVel.x, Time.fixedDeltaTime * 5f));;
    }

    /// <summary>
    /// Initializes this script for use. Should only be called once.
    /// </summary>
    private void NetworkFirstInitialize()
    {
        _characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        _fna = GetComponent<FlexNetworkAnimator>();
        _lookatIk = GetComponent<LookAtIK>();
        networkAbilityHandler = GetComponent<NetworkAbilityHandler>();
        networkBuffHandler = GetComponent<NetworkBuffHandler>();
        // if (!base.isServer && !base.hasAuthority)
        // _characterController.enabled = false;
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
                TimingStepChange = timingStepChange,
                IsSlowed = networkBuffHandler.IsSlowed
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

    private void ProcessMovementActionCodes(ClientPlayerMotorState motorState)
    {
        if (networkAbilityHandler.IsAttacking) return;
        
        switch (motorState.movementActionCode)
        {
            case MovementActionCode.Block:
            {
                if (MoveData.CanBlockOrDodge && Time.time > networkAbilityHandler.abilityClearedTime + 0.25f)
                {
                    MoveData.MovementVelocityDamp -= 0.3f;
                    animator.SetBool("Block", MoveData.CanBlockOrDodge);
                    MoveData.IsBlocking = true;
                }
                break;
            }
            case MovementActionCode.Dodge:
            {
                if (MoveData.CanBlockOrDodge && Time.time > networkAbilityHandler.abilityClearedTime + 0.25f)
                {
                    MoveData.DodgeVelocity = MoveData.MovementInput.normalized.magnitude < 0.1f ? transform.forward * (MoveData.DodgeForce * 1.2f)
                        : MoveData.MovementInput.normalized * MoveData.DodgeForce;
                    _fna.SetTrigger("Dodge");
                    AnimationTriggers.Add(new AnimationTrigger("Dodge"));
                }
                break;
            }
            default:
                animator.SetBool("Block", false);
                MoveData.IsBlocking = false;
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
        
        MoveData.MovementInput = new Vector3(motorState.Horizontal, 0f, motorState.Forward);
        MoveData.MouseDelta = motorState.MousePosition - transform.position;
        MoveData.MouseDelta.y = 0f;

        HandlePlayerMovement(motorState);
        HandlePlayerRotation(motorState.MousePosition);
       // HandlePlayerAbility(motorState.ActionCode);
        
        MoveData.Velocity = _characterController.velocity;
        MoveData.RelativeVelocity = transform.InverseTransformDirection(_characterController.velocity);
        
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
        MovementActionCode ac = InputData.GetAndResetMovementActionCode();

        ClientPlayerMotorState state = new ClientPlayerMotorState
        {
            FixedFrame = FixedUpdateManager.FixedFrame,
            Horizontal = horizontal,
            Forward = forward,
            movementActionCode = ac,
            MousePosition = CameraManager.RayMouseHit.point,
            ActiveAbility = networkAbilityHandler.IsAttacking,
            IsSlowed = networkBuffHandler.IsSlowed
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