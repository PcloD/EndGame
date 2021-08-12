using System;
using System.Collections;
using FirstGearGames.Mirrors.Assets.FlexNetworkAnimators;
using Mirror;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Serialization;


public class QueuedAbilityData
{
    public readonly float startTime;
    public readonly AbilityActionCode abilityActionCode;
    public readonly Vector3 abilityTarget;
    public readonly AbilityScriptableObject abilityScriptableObject;
    public float currentCastTime;
    public bool hasFired;

    public bool initilized = false;

    public QueuedAbilityData(AbilityActionCode actionCode, Vector3 target, AbilityScriptableObject abilScriptableObject)
    {
        startTime = Time.time;
        abilityActionCode = actionCode;
        abilityTarget = target;
        abilityScriptableObject = abilScriptableObject;
    }
}


public class NetworkAbilityHandler : NetworkBehaviour
{
    public QueuedAbilityData currentAbility = null;
    public QueuedAbilityData groundTargetAbility = null;
    public AbilityProjectile testAbilityProjectile;
    public AbilityMelee testAbilityMelee;
    public AbilityGroundProjectile testAbilityGroundProjectile;
    private double lastAttackTime;
    private int currentLightAttackIndex;
    
    private PlayerNetworkBehaviour playerNb;
    private Animator animator;
    private FlexNetworkAnimator fna;
    
    public PlayerGroundTarget GroundTargetPrefab;
    private PlayerGroundTarget groundTarget;
    
    [ReadOnly]public float abilityClearedTime;

    [FormerlySerializedAs("CurrentWeaponData")] public WeaponScriptableObject currentWeaponScriptableObject;

    public bool IsAttacking => currentAbility != null;
    
    public override void OnStartServer()
    {
        base.OnStartServer();
        Initilize();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        Initilize();

        
    }

    void Start()
    {
        if (hasAuthority)
        {
            groundTarget = Instantiate(GroundTargetPrefab);
            groundTarget.Disable();
        }
    }
    
    public void SendAbility()
    {
        if (!hasAuthority) return;
        if (!playerNb.InputData.HaveAbilityInput) return;
        
        // send then reset
        AbilityActionCode ac = playerNb.InputData.GetAndResetAbilityActionCode();
        ProcessAbilityInputs(ac, playerNb.MoveData.MouseDelta);
    }

    private void Update()
    {
        if (hasAuthority) OnClientUpdate();
        if(hasAuthority || isServer) ProcessCurrentAbility();
    }

    private void Initilize()
    {
        playerNb = GetComponent<PlayerNetworkBehaviour>();
        animator = GetComponent<Animator>();
        fna = GetComponent<FlexNetworkAnimator>();
    }

    private void ProcessAbilityInputs(AbilityActionCode abilityActionCode, Vector3 forward)
    {
        // dont process if already processing another ability
        if (IsAttacking || playerNb.MoveData.IsRolling || playerNb.MoveData.IsBlocking) return;

        switch (abilityActionCode)
        {
            
            case AbilityActionCode.None:
                break;
            case AbilityActionCode.LightAttack:
                if (Time.time < lastAttackTime + currentWeaponScriptableObject.LightAttacks[currentLightAttackIndex].LightAttackComboReset)
                {
                    // inside combo time
                    currentLightAttackIndex++;
                    if (currentLightAttackIndex >= currentWeaponScriptableObject.LightAttacks.Length) currentLightAttackIndex = 0;
                }
                else
                {
                    // outside combo time, reset count
                    currentLightAttackIndex = 0;
                }
                lastAttackTime = Time.time;
                currentAbility = new QueuedAbilityData(AbilityActionCode.LightAttack, playerNb.MoveData.MouseDelta, currentWeaponScriptableObject.LightAttacks[currentLightAttackIndex]);
                break;
            case AbilityActionCode.HeavyAttack:
                lastAttackTime = Time.time;
                currentAbility = new QueuedAbilityData(AbilityActionCode.HeavyAttack, playerNb.MoveData.MouseDelta, currentWeaponScriptableObject.HeavyAttack);
                break;
            case AbilityActionCode.Ability1:
                var ability1 = currentWeaponScriptableObject.Spell1;

                if (ability1.groundTarget)
                {
                    groundTargetAbility = new QueuedAbilityData(AbilityActionCode.Ability1, playerNb.MoveData.MouseDelta,ability1);
                    return;
                }
                lastAttackTime = Time.time;
                currentAbility = new QueuedAbilityData(AbilityActionCode.Ability1, playerNb.MoveData.MouseDelta,ability1);
                break;
            case AbilityActionCode.Ability2:
                var ability2 = currentWeaponScriptableObject.Spell2;

                if (ability2.groundTarget)
                {
                    groundTargetAbility = new QueuedAbilityData(AbilityActionCode.Ability2, playerNb.MoveData.MouseDelta,ability2);
                    return;
                }
                lastAttackTime = Time.time;
                currentAbility = new QueuedAbilityData(AbilityActionCode.Ability2, playerNb.MoveData.MouseDelta, currentWeaponScriptableObject.Spell2);
                break;
            case AbilityActionCode.Ability3:
                var ability3 = currentWeaponScriptableObject.Spell3;

                if (ability3.groundTarget)
                {
                    groundTargetAbility = new QueuedAbilityData(AbilityActionCode.Ability3, playerNb.MoveData.MouseDelta,ability3);
                    return;
                }
                lastAttackTime = Time.time;
                currentAbility = new QueuedAbilityData(AbilityActionCode.Ability3, playerNb.MoveData.MouseDelta, currentWeaponScriptableObject.Spell3);
                break;
            default:
                break;
        }
        
        if(!isServer)CmdSendAbilityInputs(abilityActionCode, playerNb.MoveData.MouseDelta);
    }



    [Command(channel = 0)]
    private void CmdSendAbilityInputs(AbilityActionCode abilityActionCode, Vector3 forward)
    {  
        ProcessAbilityInputs(abilityActionCode,forward);
    }
    /*
    [TargetRpc(channel = 0)]
    private void TargetServerSendAbility(NetworkConnection conn, QueuedAbilityData ability)
    {
       animator.SetTrigger("LightAttack"+ability.attackIndex);
       playerNb.AnimationTriggers.Add(new AnimationTrigger("LightAttack"+ability.attackIndex));
       currentAbility = new QueuedAbilityData(ability);
    }
    */

    private AbilityScriptableObject GetAbilityData(AbilityActionCode actionCode)
    {
        switch (actionCode)
        {
            case AbilityActionCode.None:
                return null;
                break;
            case AbilityActionCode.LightAttack:
                return currentWeaponScriptableObject.LightAttacks[currentLightAttackIndex];
                break;
            case AbilityActionCode.HeavyAttack:
                return currentWeaponScriptableObject.HeavyAttack;
                break;
            case AbilityActionCode.Ability1:
                return currentWeaponScriptableObject.Spell1;
                break;
            case AbilityActionCode.Ability2:
                return currentWeaponScriptableObject.Spell2;
                break;
            case AbilityActionCode.Ability3:
                return currentWeaponScriptableObject.Spell3;
                break;
            default:
                return null;
        }
    }

    [Command(channel = 0)]
    private void CmdSendGroundTarget(AbilityActionCode abilityActionCode, Vector3 targetPoint)
    {
        lastAttackTime = Time.time;
        currentAbility = new QueuedAbilityData(abilityActionCode,targetPoint, GetAbilityData(abilityActionCode));
    }
    
    private void OnClientUpdate()
    {
        if (groundTargetAbility == null)
        {
            if (PlayerGroundTarget.Instance.IsActive) PlayerGroundTarget.Instance.Disable();
            return;
        }

        if (!PlayerGroundTarget.Instance.IsActive)
            PlayerGroundTarget.Instance.Enable(CameraManager.RayMouseHit.point, 1f);

        PlayerGroundTarget.Instance.SetPosition(CameraManager.RayMouseHit.point);

        // confirm the spell
        if (Input.GetMouseButtonDown(0))
        {
            // create a new ability to reset the time
            currentAbility = new QueuedAbilityData(groundTargetAbility.abilityActionCode, CameraManager.RayMouseHit.point, groundTargetAbility.abilityScriptableObject);
            CmdSendGroundTarget(currentAbility.abilityActionCode, CameraManager.RayMouseHit.point);
            
            groundTargetAbility = null;
            PlayerGroundTarget.Instance.Disable();
        }

        // cancel the spell
        if (Input.GetMouseButtonDown(1))
        {
            groundTargetAbility = null;
            PlayerGroundTarget.Instance.Disable();
        }
    }
    
    /// <summary>
    /// Run on both Local Client & Server
    /// </summary>
    private void ProcessCurrentAbility()
    {
        if (currentAbility == null) return;
        
        var abilityData = currentAbility.abilityScriptableObject;
        var spellAbilityData = currentAbility.abilityScriptableObject as SpellAbilityScriptableObject;
        
        float castTime = 0f;
        
        if (!currentAbility.initilized)
        {
            if (spellAbilityData != null && spellAbilityData.CastTime > 0)
            {
                animator.SetBool("Casting", true);
            }
            else
            {
                // set animation values
                if (isServer) fna.SetTrigger(abilityData.AnimationName);
                if (isClient && !isServer)
                {
                    animator.SetTrigger(abilityData.AnimationName);
                    playerNb.AnimationTriggers.Add(new AnimationTrigger(abilityData.AnimationName));
                }
            }

            currentAbility.initilized = true;
        }

        if (spellAbilityData != null)
        {
            // handle casting
            if (spellAbilityData.CastTime > 0)
            {
                castTime = spellAbilityData.CastTime;
                if (currentAbility.currentCastTime <= spellAbilityData.CastTime)
                {
                    currentAbility.currentCastTime = Time.time - currentAbility.startTime;
                    animator.SetBool("Casting", true);
                    
                    // check if broken casting
                    if (CheckCastingBreak()) return;
                }
                else
                {
                    animator.SetBool("Casting", false);
                }
            }
        }
        // check if ability should deal damage / fire projectile
        if (Time.time > currentAbility.startTime + abilityData.AttackTime + castTime)
        {
            if (!currentAbility.hasFired)
            {
                currentAbility.hasFired = true;
                // deal dmg / spawn projectile 
                if (spellAbilityData != null && spellAbilityData.projectile)
                {
                    if (!isServer)
                    {
                        AbilityProjectile abilityProjectile = Instantiate(testAbilityProjectile, transform.position + Vector3.up, Quaternion.identity);
                        abilityProjectile.Initilize(0f,false, 10f, currentAbility.abilityTarget, transform);
                    }
                    if(hasAuthority) CmdSpawnProjectile(transform.position + Vector3.up, NetworkTime.time, currentAbility.abilityTarget);
                }

                if (spellAbilityData != null && spellAbilityData.groundTarget)
                {
                    if (!isServer)
                    {
                        AbilityGroundProjectile projectile = Instantiate(testAbilityGroundProjectile, transform.position, Quaternion.identity);
                        projectile.Initilize(0f,false, 10f, currentAbility.abilityTarget,transform);
                    }
                    if(hasAuthority) CmdSpawnGroundProjectile(transform.position, NetworkTime.time, currentAbility.abilityTarget);
                }

                // no spellAbilityData so must be a melee attack
                if (!spellAbilityData)
                {
                    if (!isServer)
                    {
                        AbilityMelee abilityMelee = Instantiate(testAbilityMelee, transform.position, Quaternion.identity);
                        abilityMelee.Initilize(10f,transform, false);
                    }
                    if(hasAuthority)  CmdSpawnMeleeAbility(transform.position);
                }
            }
        }

        // check if we should clear current ability
        if (Time.time > currentAbility.startTime + abilityData.AttackDuration + castTime)
        {
            currentAbility = null;
            abilityClearedTime = Time.time;
        }
    }

    private bool CheckCastingBreak()
    {
        if (playerNb.MoveData.MovementInput.sqrMagnitude > 0)
        {
            animator.SetBool("Casting", false);
            currentAbility = null;
            abilityClearedTime = Time.time;
            return true;
        }

        return false;
    }

    [Command]
    private void CmdSpawnProjectile(Vector3 position, double networkTime, Vector3 direction)
    {
        /* Determine how much time has passed between when the client
             * called the command and when it was received. */
        double timePassed = NetworkTime.time - networkTime;
        
        AbilityProjectile p = Instantiate(testAbilityProjectile, position, Quaternion.identity);
        p.Initilize((float)timePassed,true, 10f, direction, transform);
        
        RpcSpawnProjectile(position, networkTime, direction);
    }

    [ClientRpc(includeOwner = false)]
    private void RpcSpawnProjectile(Vector3 position, double networkTime, Vector3 direction)
    {
        double timePassed = NetworkTime.time - networkTime;

        AbilityProjectile p = Instantiate(testAbilityProjectile, position, Quaternion.identity);
        p.Initilize((float)timePassed,false, 10f, direction,transform);
    }
    
    [Command]
    private void CmdSpawnGroundProjectile(Vector3 position, double networkTime, Vector3 targetPoint)
    {
        /* Determine how much time has passed between when the client
             * called the command and when it was received. */
        double timePassed = NetworkTime.time - networkTime;
        
        AbilityGroundProjectile p = Instantiate(testAbilityGroundProjectile, position, Quaternion.identity);
        p.Initilize((float)timePassed,true, 10f, targetPoint,transform);
        
        RpcSpawnGroundProjectile(position, networkTime, targetPoint);
    }

    [ClientRpc(includeOwner = false)]
    private void RpcSpawnGroundProjectile(Vector3 position, double networkTime, Vector3 targetPoint)
    {
        double timePassed = NetworkTime.time - networkTime;

        AbilityGroundProjectile p = Instantiate(testAbilityGroundProjectile, position, Quaternion.identity);
        p.Initilize((float)timePassed,false, 10f, targetPoint,transform);
    }
    
    [Command]
    private void CmdSpawnMeleeAbility(Vector3 position)
    {
        
        AbilityMelee p = Instantiate(testAbilityMelee, position, Quaternion.identity);
        p.Initilize(10,transform, true);
        
        RpcSpawnMeleeAbility(position);
    }

    [ClientRpc(includeOwner = false)]
    private void RpcSpawnMeleeAbility(Vector3 position)
    {
        AbilityMelee p = Instantiate(testAbilityMelee, position, Quaternion.identity);
        p.Initilize(10, transform, false);
    }
}
