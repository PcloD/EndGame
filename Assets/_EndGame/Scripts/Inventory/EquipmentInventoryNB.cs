using Mirror;
using UnityEngine;

public class EquipmentInventoryNB : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnWeaponChanged))]
    private int WeaponID;

    public Transform WeaponEquipTransform;

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
            _currentWeapon.Initilize();
        }
    }

    private EntityAbilityHandler entityAbilityHandler;
    private WeaponScriptableObject currentWeaponScriptableObject;

    void Awake()
    {
        entityAbilityHandler = GetComponent<EntityAbilityHandler>();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        if(hasAuthority)EquipWeapon(1);
    }

    [Command]
    public void EquipWeapon(int id)
    {
        if(!isClient) OnWeaponChanged(WeaponID, id);
        WeaponID = id;
    }

    void OnWeaponChanged(int oldVal, int newVal)
    {
        currentWeaponScriptableObject = GameArmoryManager.WeaponScriptableObjects[newVal];
        CurrentWeapon = currentWeaponScriptableObject.Weapon;

        if (!isServer) return;
        entityAbilityHandler.entityTracker.AttackRange = currentWeaponScriptableObject.AutoAttackRange;
    }

}
