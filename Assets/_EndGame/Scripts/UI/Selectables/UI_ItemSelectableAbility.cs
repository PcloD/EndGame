using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_ItemSelectableAbility : UI_ItemSelectable
{
    public UI_ItemDragReference dragReference;
    public Image SpellIcon;
    public TMP_Text SpellName;
    public int SpellId;
    [ReadOnly]public SpellAbilityScriptableObject abilitySo;


    public void Init(SpellAbilityScriptableObject abilityScriptableObject, int spellId)
    {
        abilitySo = abilityScriptableObject;
        SpellIcon.sprite = abilitySo.SpellSprite;
        SpellName.text = abilitySo.AbilityName;
        SpellId = spellId;
    }
    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);
        var dragRef = Instantiate(dragReference, Input.mousePosition, Quaternion.identity,transform.root);
        dragRef.Init(abilitySo.SpellSprite);
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);
        var spellSocket = UI_Socket.currentHoverSocket as UI_SocketAbility;

        if (spellSocket)
        {
            NetworkPlayerBehaviour.Instance.equipmentInventory.CmdEquipAbility(spellSocket.abilityNetworkSocket.socketIndex, SpellId);
        }
        
        if (UI_ItemDragReference.CurrentDragReference != null)
        {
            Destroy(UI_ItemDragReference.CurrentDragReference.gameObject);
        }
    }
}
