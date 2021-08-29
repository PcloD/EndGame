using System;
using System.Collections;
using System.Collections.Generic;
using FirstGearGames.Utilities.Objects;
using UnityEngine;

public class PlayerGroundTarget : MonoBehaviour
{
    private Transform _transform;
    [SerializeField] private Transform arrowTransform;
    [SerializeField] private Transform circleTransform;
    
    public static PlayerGroundTarget Instance;
    public bool IsActive;

    private bool arrowActive;
    private bool circleActive;

    private AbilityScriptableObject _currentAbilityScriptableObject;
    public AbilityScriptableObject CurrentAbilityScriptableObject
    {
        get => _currentAbilityScriptableObject;
        set
        {
            _currentAbilityScriptableObject = value;
            if (value == null)
            {
                Disable();
                return;
            }

            switch (_currentAbilityScriptableObject.AbilityType)
            {
                case AbilityType.Instant:
                    return;
                    break;
                case AbilityType.Target:
                    return;
                    break;
                case AbilityType.SkillShot:
                    EnableArrow();
                    break;
                case AbilityType.GroundTarget:
                    EnableCircle();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    private void Update()
    {
        if (arrowActive)
        {
            var arrowDelta = CameraManager.RayMouseHit.point - _transform.position;
            SetArrowRotation(arrowDelta);
        }

        if (circleActive)
        {
            SetCirclePosition(CameraManager.RayMouseHit.point);
        }
    }

    void Awake()
    {
        Instance = this;
        _transform = GetComponent<Transform>();
        Disable();
    }

    public void SetCirclePosition(Vector3 position)
    {
        circleTransform.transform.position = position;
    }

    private void SetArrowRotation(Vector3 lookDelta)
    {
        lookDelta.y = 0f;
        _transform.rotation = Quaternion.LookRotation(lookDelta);
    }

    private void EnableCircle()
    {
        circleTransform.gameObject.SetActive(true);
        circleTransform.SetScale(new Vector3(CurrentAbilityScriptableObject.AbilitySize, CurrentAbilityScriptableObject.AbilitySize, 0.1f));
        IsActive = true;
        circleActive = true;
    }

    private void EnableArrow()
    {
        arrowTransform.gameObject.SetActive(true);
        IsActive = true;
        arrowActive = true;
    }

    private void Disable()
    {
        circleTransform.gameObject.SetActive(false);
        arrowTransform.gameObject.SetActive(false);
        IsActive = false;
        arrowActive = false;
        circleActive = false;
    }
    
    
}
