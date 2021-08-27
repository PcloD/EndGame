using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class AbilityPanel : MonoBehaviour
{
    public Transform CanvasTransform;
    public Transform ContentTransform;
    public UI_ItemSelectableAbility SelectableAbilityPrefab;
    
    private List<UI_ItemSelectableAbility> uiAbilities = new List<UI_ItemSelectableAbility>();
    public GameObject Panel;
    private bool isOpen;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (isOpen)
            {
                ClosePanel();
            }
            else
            {
                OpenPanel();;
            }
        }
    }

    void Start()
    {
        Init();
    }

    private void OpenPanel()
    {
        Panel.SetActive(true);
        isOpen = true;
    }

    void Init()
    {
        foreach (var abilitySpellScriptableObject in GameArmoryManager.AbilitySpellScriptableObjects.Values)
        {
            if (abilitySpellScriptableObject is SpellAbilityScriptableObject)
            {
                var uiSpell = Instantiate(SelectableAbilityPrefab, Vector3.zero, Quaternion.identity, ContentTransform);
                uiSpell.Init(abilitySpellScriptableObject as SpellAbilityScriptableObject, abilitySpellScriptableObject.AbilityId, CanvasTransform);
                uiAbilities.Add(uiSpell);
            }
        }
    }

    private void ClosePanel()
    {
        Panel.SetActive(false);
        isOpen = false;
    }
}
