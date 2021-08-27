using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Socket : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public static UI_Socket currentHoverSocket;

    public Image IconImage;
    
    protected Sprite startSprite;

    void Awake()
    {
        IconImage.enabled = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        currentHoverSocket = this;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        currentHoverSocket = null;
    }
}
