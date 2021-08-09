using System.Collections;
using System.Collections.Generic;
using FirstGearGames.Utilities.Objects;
using UnityEngine;

public class PlayerGroundTarget : MonoBehaviour
{
    private Transform _transform;
    [SerializeField]private Transform _meshTransform;

    public static PlayerGroundTarget Instance;
    public bool IsActive;

    void Awake()
    {
        Instance = this;
        _transform = GetComponent<Transform>();
        Disable();
    }

    public void SetPosition(Vector3 position)
    {
        _transform.position = position;
    }

    public void Enable(Vector3 position, float size)
    {
        SetPosition(position);
        _transform.SetScale(Vector3.one * size);
        _meshTransform.gameObject.SetActive(true);
        IsActive = true;
    }

    public void Disable()
    {
        _meshTransform.gameObject.SetActive(false);
        IsActive = false;
    }
    
    
}
