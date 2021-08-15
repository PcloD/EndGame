using System;
using System.Collections;
using System.Collections.Generic;
using FirstGearGames.Mirrors.Assets.FlexNetworkAnimators;
using Mirror;
using Pathfinding;
using Unity.Mathematics;
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
    public WeaponScriptableObject TESTWEAPON;
    public EntityEnemyTracker entityTracker;

    private NetworkPlayerBehaviour playerNb;
    private Animator animator;
    private FlexNetworkAnimator fna;
    
    private WeaponScriptableObject currentWeaponScriptableObject;
    private int _currentWeaponScriptableObjectIndex;
    public int currentWeaponScriptableObjectIndex
    {
        get { return _currentWeaponScriptableObjectIndex; }
        set
        {
            _currentWeaponScriptableObjectIndex = value;
            currentWeaponScriptableObject = GameArmoryManager.WeaponScriptableObjects[value];
            entityTracker.AttackRange = currentWeaponScriptableObject.AutoAttackRange;
        }
    }

    private CurrentAbility _currentAbility;
    public CurrentAbility currentAbility
    {
        get { return _currentAbility; }
        set
        {
            _currentAbility = value;
            
            if(_currentAbility == null) fna.SetTrigger("AbortAbility");
            
            if (_currentAbility != null)
            {
                playerNb.aStar.isStopped = true;
            }
        }
    }
    private float lastAutoTime;


    
    private bool IsAutoAttackAvailable()
    {
        return lastAutoTime < Time.time + currentWeaponScriptableObject.AutoAttacks[currentAutoIndex].AttackDuration;
    }

    public void Update()
    {
        if (!isServer) return;
        
        
        TryAutoAttack();
        ProcessCurrentAbility();
        entityTracker.OnUpdate();
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        entityTracker = new EntityEnemyTracker(transform, GetComponent<AIPath>(), this);
    }

    void Start()
    {
        currentWeaponScriptableObjectIndex = GameArmoryManager.FindWeaponScriptableObjectIndex(TESTWEAPON);
        playerNb = GetComponent<NetworkPlayerBehaviour>();
        fna = GetComponent<FlexNetworkAnimator>();
        animator = GetComponent<Animator>();
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
        if (Time.time < lastAutoTime + currentWeaponScriptableObject.AutoAttacks[currentAutoIndex].LightAttackComboReset)
        {
            // inside combo time
            currentAutoIndex++;
            if (currentAutoIndex >= currentWeaponScriptableObject.AutoAttacks.Length) currentAutoIndex = 0;
        }

        lastAutoTime = Time.time;
        currentAbility = new CurrentAbility(AbilityCode.Auto, currentWeaponScriptableObject.AutoAttacks[currentAutoIndex]);
    }

    [Server]
    private void ProcessCurrentAbility()
    {
        if (currentAbility == null) return;
        
        // check if player has aborted ability by moving
        if (playerNb.IsMoving())
        {
            Debug.Log($"moving so killing ability");
            currentAbility = null;
            return;
        }

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
        
        TryFireAbility();
        TryClearAbility();
    }

    private void TryFireAbility()
    {
        if (currentAbility.hasFired) return;

        if (Time.time > currentAbility.startTime + currentAbility.abilitySo.AttackTime)
        {
            currentAbility.hasFired = true;

            switch (currentAbility.abilityCode)
            {
                case AbilityCode.Auto:
                    switch (currentWeaponScriptableObject.AutoHitStyle)
                    {
                        case AutoHitStyle.Melee:
                            Debug.Log($"Server Damage for 5!");
                            // deal melee damage
                            break;
                        case AutoHitStyle.Ranged:
                            // rpc ranged attack
                            var directProjectile = Instantiate(currentAbility.abilitySo.AbilityPrefab, transform.position + Vector3.up, quaternion.identity) as AbilityDirectProjectile;
                            directProjectile.Initilize(true, 5f, entityTracker.TrackedEnemyTransform.position + Vector3.up, transform);
                            
                            RpcAutoAttack(entityTracker.TrackedEnemyTransform.gameObject, currentAutoIndex);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
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

            // spawn ability and RPC to everyone
           // Ability ability = Instantiate(currentAbility.abilitySo.AbilityPrefab, transform.position, Quaternion.identity);
          //  ability.Initilize(10f, transform, true);
        }
    }

    private void TryClearAbility()
    {
        if (Time.time > currentAbility.startTime + currentAbility.abilitySo.AttackDuration)
        {
            currentAbility = null;
        }
    }



    [ClientRpc]
    private void RpcAutoAttack(GameObject target, int comboCount)
    {
        var directProjectile = Instantiate(currentWeaponScriptableObject.AutoAttacks[comboCount].AbilityPrefab, transform.position + Vector3.up, quaternion.identity) as AbilityDirectProjectile;
        
        directProjectile.Initilize(true, 5f, entityTracker.TrackedEnemyTransform.position + Vector3.up, transform);
                            
    }
}

public class CurrentAbility
{
    public readonly float startTime;
    public readonly AbilityCode abilityCode;
    public readonly AbilityScriptableObject abilitySo;

    public bool initilized = false;
    public bool hasFired = false;

    public CurrentAbility(AbilityCode ac, AbilityScriptableObject abSo)
    {
        abilityCode = ac;
        startTime = Time.time;
        abilitySo = abSo;
    }
}
