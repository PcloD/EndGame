using System;
using TMPro;
using UnityEngine;

public class CombatTextPopup : MonoBehaviour
{
    private TMP_Text textAsset;
    private float endTime;
    private Vector3 endPos;

    public void Display(int dmgAmount, TMP_Style style)
    {
        textAsset = GetComponent<TMP_Text>();
        textAsset.textStyle = style;

        endTime = Time.timeSinceLevelLoad + CombatTextManager.Instance.VisibleDuration;
        endPos = transform.position + CombatTextManager.Instance.DestinationOffset;
        
        textAsset.SetText($"{style.styleOpeningDefinition}{dmgAmount.ToString()}{style.styleClosingDefinition}");
    }

    private void Update()
    {
        if(Time.timeSinceLevelLoad > endTime)
            Destroy(gameObject);
        
        transform.position = Vector3.Lerp(transform.position, endPos,
            CombatTextManager.Instance.VisibleDuration * Time.deltaTime);
    }
}