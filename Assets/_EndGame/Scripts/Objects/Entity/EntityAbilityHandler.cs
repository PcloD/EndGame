using System;
using System.Collections;
using System.Collections.Generic;
using System.Management.Instrumentation;
using System.Runtime.CompilerServices;
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
    Spell3 = 4, 
    Spell4 = 5
}

public class EntityAbilityHandler : NetworkBehaviour
{
    public EntityEnemyTracker entityTracker;

    private NetworkPlayerBehaviour playerNb;
    private Animator animator;
    private FlexNetworkAnimator fna;
    private EquipmentInventoryNB equipmentInventory;
    private bool playAbortAnim = false;
   
    private CurrentAbility _currentAbility;
    public CurrentAbility currentAbility
    {
        get { return _currentAbility; }
        set
        {
            _currentAbility = value;
            if (_currentAbility == null)
            {
                animator.SetBool("Casting", false);
            }
            if (playAbortAnim && _currentAbility == null)
            {
                fna.SetTrigger("AbortAbility");
                playAbortAnim = false;
            }
            
            if (_currentAbility != null)
            {
                playerNb.aStar.isStopped = true;
            }

            playerNb.CurrentClientAbility = value;
        }
    }
    private double lastAutoTime;
    private bool IsAutoAttackAvailable()
    {
        return lastAutoTime < NetworkTime.time + equipmentInventory.CurrentWeapon.WeaponScriptableObject.AutoAttacks[currentAutoIndex].AttackDuration;
    }

    public void Update()
    {
        if (hasAuthority) ClientAbilities();
        
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

    void Awake()
    {
        playerNb = GetComponent<NetworkPlayerBehaviour>();
        fna = GetComponent<FlexNetworkAnimator>();
        animator = GetComponent<Animator>();
        equipmentInventory = GetComponent<EquipmentInventoryNB>();
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
        if (NetworkTime.time < lastAutoTime + equipmentInventory.CurrentWeapon.WeaponScriptableObject.AutoAttacks[currentAutoIndex].AutoAttackComboReset)
        {
            // inside combo time
            currentAutoIndex++;
            if (currentAutoIndex >= equipmentInventory.CurrentWeapon.WeaponScriptableObject.AutoAttacks.Length) currentAutoIndex = 0;
        }

        lastAutoTime = NetworkTime.time;
        currentAbility = new CurrentAbility(AbilityCode.Auto, equipmentInventory.CurrentWeapon.WeaponScriptableObject.AutoAttacks[currentAutoIndex].AbilityId);
    }

    [Server]
    private void ProcessCurrentAbility()
    {
        if (currentAbility == null) return;
        
        // check if player has aborted ability by moving
        if (playerNb.IsMoving())
        {
            Debug.Log($"moving so killing ability");
            playAbortAnim = true;
            currentAbility = null;
            return;
        }

        if (!currentAbility.initilized)
        {
            switch (currentAbility.abilityCode)
            {
                case AbilityCode.Auto:
                    fna.SetTrigger(currentAbility.AbilitySO.GetAnimationString());
                    break;
                case AbilityCode.Spell1:
                case AbilityCode.Spell2:
                case AbilityCode.Spell3:
                case AbilityCode.Spell4:
                    if (currentAbility.AbilitySO.CastTime > 0)
                    {
                        animator.SetBool("Casting", true);
                    }
                    else
                    {
                        fna.SetTrigger(currentAbility.AbilitySO.GetAnimationString());
                    }
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

        if (NetworkTime.time > currentAbility.startTime + currentAbility.AbilitySO.AttackTime + currentAbility.AbilitySO.CastTime)
        {
            currentAbility.hasFired = true;
            animator.SetBool("Casting", false);
            switch (currentAbility.abilityCode)
            {
                case AbilityCode.Auto:
                    switch (equipmentInventory.CurrentWeapon.WeaponScriptableObject.AutoHitStyle)
                    {
                        case AutoHitStyle.Melee:
                            Debug.Log($"Server Damage for 5!");
                            // deal melee damage
                            break;
                        case AutoHitStyle.Ranged:
                            // rpc ranged attack
                            var directProjectile = Instantiate(currentAbility.AbilitySO.AbilityPrefab, transform.position + Vector3.up, quaternion.identity) as AbilityTarget;
                            directProjectile.Initilize(true, 5f, entityTracker.TrackedEnemyTransform.gameObject, playerNb);
                            
                            RpcAutoAttack(entityTracker.TrackedEnemyTransform.gameObject, currentAutoIndex);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    break;
                case AbilityCode.Spell1:
                case AbilityCode.Spell2:
                case AbilityCode.Spell3:
                case AbilityCode.Spell4:
                    switch (currentAbility.AbilitySO.AbilityType)
                    {
                        case AbilityType.Instant:
                            break;
                        case AbilityType.Target:
                            break;
                        case AbilityType.SkillShot:
                            AbilitySkillShot skillShot = Instantiate(currentAbility.AbilitySO.AbilityPrefab, transform.position + Vector3.up, Quaternion.identity) as AbilitySkillShot;
                            skillShot.Initilize(true,5f,transform.forward, playerNb, 5f);
                            
                            RpcAbility(transform.forward, currentAbility.abilityCode);
                            break;
                        case AbilityType.GroundTarget:
                            break;
                        default:
                            break;
                    }
                  

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    private void TryClearAbility()
    {
        if (NetworkTime.time > currentAbility.startTime + currentAbility.AbilitySO.AttackDuration + currentAbility.AbilitySO.CastTime)
        {
            currentAbility = null;
        }
    }



    [ClientRpc]
    private void RpcAutoAttack(GameObject target, int comboCount)
    {
        // dont run on host
        if (isServer) return;
        
        var directProjectile = Instantiate(equipmentInventory.CurrentWeapon.WeaponScriptableObject.AutoAttacks[comboCount].AbilityPrefab, transform.position + Vector3.up, quaternion.identity) as AbilityTarget;
        
        directProjectile.Initilize(false, 5f, target, playerNb);
    }

    [ClientRpc]
    private void RpcAbility(Vector3 forward, AbilityCode abilityCode)
    {
        // dont run on host
        if (isServer) return;

        var spellSO = GetAbilitySO(abilityCode);
        switch (spellSO.AbilityType)
        {
            case AbilityType.Instant:
                break;
            case AbilityType.Target:
                break;
            case AbilityType.SkillShot:
                var skillShotProjectile = Instantiate(spellSO.AbilityPrefab, transform.position + Vector3.up, Quaternion.identity) as AbilitySkillShot;
                
                skillShotProjectile.Initilize(false,5f,forward, playerNb, 5f);
                break;
            case AbilityType.GroundTarget:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
    }

    AbilityScriptableObject GetAbilitySO(AbilityCode abilityCode)
    {
        switch (abilityCode)
        {
            case AbilityCode.Auto:
                Debug.LogError("Unsupported until i add combo count in here");
                return null;
                break;
            case AbilityCode.Spell1:
                if (equipmentInventory.SpellAbilitySockets[0].spellId < 0) return null;
                return GameArmoryManager.AbilitySpellScriptableObjects[equipmentInventory.SpellAbilitySockets[0].spellId];
                break;
            case AbilityCode.Spell2:
                if (equipmentInventory.SpellAbilitySockets[1].spellId < 0) return null;
                return GameArmoryManager.AbilitySpellScriptableObjects[equipmentInventory.SpellAbilitySockets[1].spellId];
                break;
            case AbilityCode.Spell3:
                if (equipmentInventory.SpellAbilitySockets[2].spellId < 0) return null;
                return GameArmoryManager.AbilitySpellScriptableObjects[equipmentInventory.SpellAbilitySockets[2].spellId];
                break;
            case AbilityCode.Spell4:
                if (equipmentInventory.SpellAbilitySockets[3].spellId < 0) return null;
                return GameArmoryManager.AbilitySpellScriptableObjects[equipmentInventory.SpellAbilitySockets[3].spellId];
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(abilityCode), abilityCode, null);
        }
    }
    
    [Client]
    void ClientAbilities()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (equipmentInventory.SpellAbilitySockets[0].spellId <= 0) return;
            CmdTryQueueAbilitity(AbilityCode.Spell1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            // do spell 2
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            // do spell 3
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            // do spell 4
        }
    }

    [Command]
    void CmdTryQueueAbilitity(AbilityCode ac)
    {
        AbilityScriptableObject spellScriptableObject = null;
        int spellId = -1;
        switch (ac)
        {
            case AbilityCode.Auto:
                Debug.LogError("Trying to queue Auto attack in wrong place");
                return;
                break;
            case AbilityCode.Spell1:
                spellScriptableObject = GameArmoryManager.AbilitySpellScriptableObjects[equipmentInventory.SpellAbilitySockets[0].spellId];
                break;
            case AbilityCode.Spell2:
                spellScriptableObject = GameArmoryManager.AbilitySpellScriptableObjects[equipmentInventory.SpellAbilitySockets[1].spellId];
                break;
            case AbilityCode.Spell3:
                spellScriptableObject = GameArmoryManager.AbilitySpellScriptableObjects[equipmentInventory.SpellAbilitySockets[2].spellId];
                break;
            case AbilityCode.Spell4:
                spellScriptableObject = GameArmoryManager.AbilitySpellScriptableObjects[equipmentInventory.SpellAbilitySockets[3].spellId];
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(ac), ac, null);
        }

        if (spellScriptableObject == null)
        {
            Debug.LogError($"Failed to find Spell server side - {ac.ToString()}");
            return;
        }
        
        currentAbility = new CurrentAbility(ac, spellScriptableObject.AbilityId);
    }
}

[Serializable]
public class CurrentAbility
{
    public readonly double startTime;
    public readonly AbilityCode abilityCode;
    public readonly int abilityId;
    
    public AbilityScriptableObject AbilitySO => GameArmoryManager.AbilitySpellScriptableObjects[abilityId];

    public bool initilized = false;
    public bool hasFired = false;
    public bool IsCastable => abilityId > 0 && AbilitySO.CastTime > 0;
    public double GetCurrentCastTime => NetworkTime.time - startTime;
    public double GetCurrentCastPercent => Mathf.Clamp((float)(NetworkTime.time - startTime / AbilitySO.CastTime),0f,1f);
    public bool IsCasting => (NetworkTime.time - startTime) < AbilitySO.CastTime;
    public bool IsCastComplete => (NetworkTime.time - startTime) > AbilitySO.CastTime;

    public CurrentAbility()
    {
        
    }

    public CurrentAbility(AbilityCode ac, int abilityId)
    {
        abilityCode = ac;
        startTime = NetworkTime.time;
        this.abilityId = abilityId;
    }
}
