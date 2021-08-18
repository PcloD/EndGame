using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_ItemSelectable : MonoBehaviour, IBeginDragHandler, IPointerEnterHandler, IDragHandler, IEndDragHandler
{
    
    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        // spawn little icon
        Debug.Log($"Drag begin");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log($"pointer enter");
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log($"Drag");
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log($"Drag end");
    }
}
