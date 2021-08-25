using System;
using Mirror;
using UnityEngine;


[Serializable]
public struct AbilityNetworkSocket
{
    public int socketIndex;
    public int spellId;
    public bool HasSpell => spellId >= 0;
}


public class EquipmentInventoryNB : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnWeaponChanged))]
    private int WeaponID = -1;
    
    public SyncList<AbilityNetworkSocket> SpellAbilitySockets = new SyncList<AbilityNetworkSocket>();
    

    public Transform WeaponEquipTransform;
    
    private EntityAbilityHandler entityAbilityHandler;
    private WeaponScriptableObject currentWeaponScriptableObject;
    
    private Weapon _currentWeapon;
    public Weapon CurrentWeapon
    {
        get
        {
            return _currentWeapon;
        }
        set
        {
            if (_currentWeapon) Destroy(_currentWeapon);
            
            _currentWeapon = Instantiate(value, WeaponEquipTransform);
            Debug.Log($"Set weapon {_currentWeapon}");
            _currentWeapon.Initilize(currentWeaponScriptableObject);
        }
    }

   

    void Awake()
    {
        entityAbilityHandler = GetComponent<EntityAbilityHandler>();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        if(hasAuthority)EquipWeapon(0);
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        // add 6 spell sockets
        for (int i = 0; i < 6; i++)
        {
            // set empty slot on init
            AbilityNetworkSocket socket = new AbilityNetworkSocket {spellId = -1, socketIndex = i};
            
            SpellAbilitySockets.Add(socket);
        }
    }

    [Command]
    public void EquipWeapon(int id)
    {
        // dont run if host mode as its called in Hook function
        if(!isClient) OnWeaponChanged(WeaponID, id);
        WeaponID = id;
    }

    [Command]
    public void CmdEquipAbility(int socketId, int spellId)
    {
        var spellAbilitySocket = new AbilityNetworkSocket
        {
            socketIndex = SpellAbilitySockets[socketId].socketIndex, spellId = spellId
        };
        SpellAbilitySockets[socketId] = spellAbilitySocket;
    }
    
    

    void OnWeaponChanged(int oldVal, int newVal)
    {
        currentWeaponScriptableObject = GameArmoryManager.WeaponScriptableObjects[newVal];
        CurrentWeapon = currentWeaponScriptableObject.Weapon;

        if (!isServer) return;
        entityAbilityHandler.entityTracker.AttackRange = currentWeaponScriptableObject.AutoAttackRange;
    }

    [ContextMenu("Set Ability 1")]
    public void SetAbility1()
    {
        var spellAbilitySocket = new AbilityNetworkSocket
        {
            socketIndex = SpellAbilitySockets[0].socketIndex, spellId = 1
        };
        SpellAbilitySockets[0] = spellAbilitySocket;
    }
}
