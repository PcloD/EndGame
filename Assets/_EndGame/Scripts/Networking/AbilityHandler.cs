using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using FirstGearGames.Mirrors.Assets.FlexNetworkAnimators;
using Mirror;
using Unity.Collections;
using UnityEngine;

/// <summary>
/// Attached to player character. Runs on all player objects for server. Just for player character for local client
/// </summary>
[Serializable]
public class AbilityHandler
{
    [Serializable]
    public class QueuedAbility
    {
        public float StartTime;
        public bool FinishedDamage = false;
        public AbilityData AbilityData;

        public QueuedAbility(AbilityData data)
        {
            StartTime = Time.time;
            AbilityData = data;
        }
    }
    
    [ReadOnly] public WeaponData CurrentWeaponData;
    [ReadOnly] public PlayerNetworkBehaviour playerNetworkBehaviour;

    private float lastLightAttackTime;
    private float lastHeavyAttackTime;
    private int currentLightAttackIndex;

    public float AbilityClearedTimed;
    public QueuedAbility CurrentAbility;

    private Animator _animator;
    private FlexNetworkAnimator _fna;

    public AbilityHandler(Animator anim, FlexNetworkAnimator fna, WeaponData weaponData, PlayerNetworkBehaviour playerNb)
    {
        _animator = anim;
        _fna = fna;
        CurrentWeaponData = weaponData;
        playerNetworkBehaviour = playerNb;

    }

    void IncrementLightAttack()
    {
        
    }

    /// <summary>
    /// Sets the animation / ability values, both server & auth client
    /// </summary>
    public void HandleAbility(ActionCodes actionCode)
    {
        // abort if already processing an ability
        if (CurrentAbility != null) return;
        
        switch (actionCode)
        {
            case ActionCodes.LightAttack:
            {
                    // determine which light attack to use
                    if (Time.time < lastLightAttackTime + CurrentWeaponData.LightAttackComboReset)
                    {
                        Debug.Log($"Inside Combo");
                        // inside combo time
                        currentLightAttackIndex++;
                        if (currentLightAttackIndex >= CurrentWeaponData.LightAttacks.Length)
                        {
                            currentLightAttackIndex = 0;
                        }
                    }
                    else
                    {
                        // outside combo time, reset count
                        currentLightAttackIndex = 0;
                    }

                    var lightAttackData = CurrentWeaponData.LightAttacks[currentLightAttackIndex];
                    
                    // set movement values
                    playerNetworkBehaviour.MoveData.AttackVelocity = lightAttackData.AttackForce * playerNetworkBehaviour.MoveData.MouseDelta.normalized;
                    playerNetworkBehaviour.AttackVelocityNegativeForce = lightAttackData.AttackNegativeForce;

                    // set animation values
                    
                    _fna.SetTrigger("LightAttack"+currentLightAttackIndex);
                    playerNetworkBehaviour.AnimationTriggers.Add(new AnimationTrigger("LightAttack"+currentLightAttackIndex));

                    // set cooldown & queue ability
                    lastLightAttackTime = Time.time;
                    CurrentAbility = new QueuedAbility(lightAttackData);
                    break;
            }
            case ActionCodes.HeavyAttack:
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// called on the servers playerNb update loop
    /// </summary>
    [Server]
    public void OnServerUpdate()
    {
        if (CurrentAbility == null) return;

        // idk unity. when i select in inspector random shit is nulled
        if (CurrentAbility.AbilityData == null)
        {
            CurrentAbility = null;
            return; 
        }
        
        // check if we should clear current ability
        if (Time.time > CurrentAbility.StartTime + CurrentAbility.AbilityData.AttackDuration)
        {
            CurrentAbility = null;
            AbilityClearedTimed = Time.time;
            return;
        }
        
        // check if we should do damage
        if (!CurrentAbility.FinishedDamage)
        {
            if (Time.time > CurrentAbility.StartTime + CurrentAbility.AbilityData.AttackTime)
            {
                // do damage
                CurrentAbility.FinishedDamage = true;
            }
        }
    }
}
