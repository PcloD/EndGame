using System;
using System.Collections;
using System.Collections.Generic;
using FirstGearGames.Mirrors.Assets.FlexNetworkAnimators;
using Mirror;
using UnityEngine;

public enum AbilityCode
{
    Auto = 1,
    Spell1 = 2,
    Spell2 = 3,
    Spell3 = 4
}

public class EntityAbilityHandler : NetworkBehaviour
{
    private const float AUTO_ATTACK_TIME = 0.3f;

    // i think we syncVar this? or maybe make a lighter struct
    public WeaponScriptableObject TESTWEAPON;
    
    private NetworkPlayerBehaviour playerNb;
    private Animator animator;
    private FlexNetworkAnimator fna;
    private EntityEnemyTracker entityTracker;
    private bool initilized = false;

    private CurrentAbility currentAbility;
    private float lastAutoTime;

    public void ServerInitilize(NetworkPlayerBehaviour playerNb, Animator animator, EntityEnemyTracker entityTracker, FlexNetworkAnimator flexNetworkAnimator)
    {
        this.playerNb = playerNb;
        this.animator = animator;
        this.entityTracker = entityTracker;
        this.fna = flexNetworkAnimator;
        this.initilized = true;
    }

    public class CurrentAbility
    {
        public readonly float startTime;
        public readonly AbilityCode abilityCode;
        public readonly AbilityScriptableObject abilitySo;

        public bool initilized = false;

        public CurrentAbility(AbilityCode ac, AbilityScriptableObject abSo)
        {
            abilityCode = ac;
            startTime = Time.time;
            abilitySo = abSo;
        }
    }
    
    private bool IsAutoAttackAvailable()
    {
        return lastAutoTime < Time.time + AUTO_ATTACK_TIME;
    }

    public void OnUpdate()
    {
        if (isServer)
        {
            TryAutoAttack();
            ProcessCurrentAbility();
        }
    }

    [Server]
    private void TryAutoAttack()
    {
        if (currentAbility != null) return;
        if (!entityTracker.TrackedEnemyTransform) return;

        if (entityTracker.IsInRange() && IsAutoAttackAvailable())
        {
            QueueAutoAttack();
        }
    }

    
    private int currentAutoIndex;
    [Server]
    private void QueueAutoAttack()
    {
        if (Time.time < lastAutoTime + TESTWEAPON.LightAttacks[currentAutoIndex].LightAttackComboReset)
        {
            // inside combo time
            currentAutoIndex++;
            if (currentAutoIndex >= TESTWEAPON.LightAttacks.Length) currentAutoIndex = 0;
        }

        lastAutoTime = Time.time;
        currentAbility = new CurrentAbility(AbilityCode.Auto, TESTWEAPON.LightAttacks[currentAutoIndex]);
    }

    [Server]
    private void ProcessCurrentAbility()
    {
        if (currentAbility == null) return;

        if (!currentAbility.initilized)
        {
            switch (currentAbility.abilityCode)
            {
                case AbilityCode.Auto:
                    fna.SetTrigger(currentAbility.abilitySo.AnimationName);
                    break;
                case AbilityCode.Spell1:
                    break;
                case AbilityCode.Spell2:
                    break;
                case AbilityCode.Spell3:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            currentAbility.initilized = true;
        }
        TryClearAbility();
    }

    private void TryClearAbility()
    {
        if (Time.time > currentAbility.startTime + currentAbility.abilitySo.AttackDuration)
        {
            currentAbility = null;
        }
    }



    [ClientRpc]
    private void RpcAutoAttack()
    {
        
    }
}
