using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Utils;
using Microsoft.Win32;
using UnityEngine;

public class MouseCursorManager : MonoBehaviour
{
    public enum CursorType
    {
        Default = 0,
        DefaultCharacter = 1,
        Attack = 2
    }

    private CursorType _currentCursorType;
    public CursorType CurrentCursorType
    {
        get { return _currentCursorType;}
        set
        {
            if (_currentCursorType == value) return;

            _currentCursorType = value;

            switch (_currentCursorType)
            {
                case CursorType.Default:
                    Cursor.SetCursor(DefaultCursor, Vector3.zero, CursorMode.Auto);
                    break;
                case CursorType.Attack:
                    Cursor.SetCursor(AttackCursor, Vector3.zero, CursorMode.Auto);
                    break;
                case CursorType.DefaultCharacter:
                    Cursor.SetCursor(DefaultCursor, Vector3.zero, CursorMode.Auto);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }
    }

    public static MouseCursorManager Instance;
    
    [SerializeField] private ParticleSystem ClickMoveParticle;
    [SerializeField] private Texture2D DefaultCursor;
    [SerializeField] private Texture2D AttackCursor;

    [ReadOnly] public GameObject EnemyEntity;
    
    private Transform clickParticleParent;

    private void Awake()
    {
        Instance = this;
        Cursor.SetCursor(DefaultCursor, Vector3.zero, CursorMode.Auto);
        
        ClickMoveParticle.Stop(true);
        clickParticleParent = ClickMoveParticle.transform.parent;
    }

    public void DoClickParticle()
    {
        clickParticleParent.up = CameraManager.RayMouseHit.normal;
        clickParticleParent.position = CameraManager.RayMouseHit.point;
        
        ClickMoveParticle.Play();
    }
}
