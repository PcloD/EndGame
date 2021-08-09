using System;
using System.Collections;
using FirstGearGames.Mirrors.Assets.FlexNetworkAnimators;
using Mirror;
using UnityEngine;


public class QueuedAbilityData
{
    public readonly float startTime;
    public readonly AbilityActionCode abilityActionCode;
    public readonly Vector3 abilityForward;
    public readonly AbilityData abilityData;
    public float currentCastTime;
    public bool hasFired;

    public bool initilized = false;

    public QueuedAbilityData(AbilityActionCode actionCode, Vector3 forward, AbilityData abilData)
    {
        startTime = Time.time;
        abilityActionCode = actionCode;
        abilityForward = forward;
        abilityData = abilData;
    }
}


public class NetworkAbilityHandler : NetworkBehaviour
{
    public QueuedAbilityData currentAbility = null;
    public Projectile TestProjectile;
    private double lastAttackTime;
    private int currentLightAttackIndex;
    
    private PlayerNetworkBehaviour playerNb;
    private Animator animator;
    private FlexNetworkAnimator fna;
    public float abilityClearedTime;

    public WeaponData CurrentWeaponData;

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
        ProcessCurrentAbility();
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
                if (Time.time < lastAttackTime + CurrentWeaponData.LightAttacks[currentLightAttackIndex].LightAttackComboReset)
                {
                    // inside combo time
                    currentLightAttackIndex++;
                    if (currentLightAttackIndex >= CurrentWeaponData.LightAttacks.Length) currentLightAttackIndex = 0;
                }
                else
                {
                    // outside combo time, reset count
                    currentLightAttackIndex = 0;
                }
                lastAttackTime = Time.time;
                currentAbility = new QueuedAbilityData(AbilityActionCode.LightAttack, playerNb.MoveData.MouseDelta, CurrentWeaponData.LightAttacks[currentLightAttackIndex]);
                break;
            case AbilityActionCode.HeavyAttack:
                lastAttackTime = Time.time;
                currentAbility = new QueuedAbilityData(AbilityActionCode.HeavyAttack, playerNb.MoveData.MouseDelta, CurrentWeaponData.HeavyAttack);
                break;
            case AbilityActionCode.Ability1:
                lastAttackTime = Time.time;
                currentAbility = new QueuedAbilityData(AbilityActionCode.Ability1, playerNb.MoveData.MouseDelta, CurrentWeaponData.Spell1);
                break;
            case AbilityActionCode.Ability2:
                lastAttackTime = Time.time;
                currentAbility = new QueuedAbilityData(AbilityActionCode.Ability2, playerNb.MoveData.MouseDelta, CurrentWeaponData.Spell2);
                break;
            case AbilityActionCode.Ability3:
                lastAttackTime = Time.time;
                currentAbility = new QueuedAbilityData(AbilityActionCode.Ability3, playerNb.MoveData.MouseDelta, CurrentWeaponData.Spell3);
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
    
    /// <summary>
    /// Run on both Local Client & Server
    /// </summary>
    private void ProcessCurrentAbility()
    {
        if (currentAbility == null) return;
        
        var abilityData = currentAbility.abilityData;
        var spellAbilityData = currentAbility.abilityData as SpellAbilityData;
        
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
        
        // check if ability has cast time
        
        if (spellAbilityData != null)
        {
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
                        Projectile projectile = Instantiate(TestProjectile, transform.position, Quaternion.identity);
                        projectile.Initilize(0f, 10f, currentAbility.abilityForward);
                    }
                    CmdSpawnProjectile(transform.position, NetworkTime.time, currentAbility.abilityForward);
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
        
        Projectile p = Instantiate(TestProjectile, position, Quaternion.identity);
        p.Initilize((float)timePassed, 10f, direction);
        
        RpcSpawnProjectile(position, networkTime, direction);
    }

    [ClientRpc(includeOwner = false)]
    private void RpcSpawnProjectile(Vector3 position, double networkTime, Vector3 direction)
    {
        double timePassed = NetworkTime.time - networkTime;

        Projectile p = Instantiate(TestProjectile, position, Quaternion.identity);
        p.Initilize((float)timePassed, 10f, direction);
    }
    
}
