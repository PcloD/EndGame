using System;
using TMPro;
using UnityEngine;

public class CombatTextPopup : MonoBehaviour
{
    private TMP_Text textAsset;
    private float endTime;
    private Vector3 endPos;
    private Vector3 endPosAlt;

    public void Display(int dmgAmount, TMP_Style style, Vector3 destPos)
    {
        textAsset = GetComponent<TMP_Text>();
        textAsset.textStyle = style;

        endTime = Time.timeSinceLevelLoad + CombatTextManager.Instance.VisibleDuration;
        endPos = transform.position + destPos;
        
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