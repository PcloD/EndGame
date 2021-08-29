using System;
using UnityEngine;

public class NamePlate : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color inactiveColor;
    [SerializeField] private Color activeColor;
    
    public bool active = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (active)
            spriteRenderer.color = activeColor;
        else
            spriteRenderer.color = inactiveColor;

    }
}