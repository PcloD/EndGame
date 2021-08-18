using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_ItemDragReference : MonoBehaviour
{
    public static UI_ItemDragReference CurrentDragReference;
    public Image SpellIconImage;

    private Transform _transform;

    void Awake()
    {
        if (CurrentDragReference)
        {
            Debug.LogError("Should only have 1 current drag reference at a time. Destroying this");
            Destroy(gameObject);
        }
        _transform = transform;
        CurrentDragReference = this;
    }

    public void Init(Sprite sprite)
    {
        SpellIconImage.sprite = sprite;
    }

    private void Update()
    {
        _transform.position = Input.mousePosition;
    }

    private void OnDestroy()
    {
        CurrentDragReference = null;
    }
}
