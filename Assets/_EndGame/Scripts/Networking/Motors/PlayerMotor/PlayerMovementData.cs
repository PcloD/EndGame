using System;
using Assets.Scripts.Utils;
using UnityEngine;

[Serializable]
public class PlayerMovementData
{
    public float BaseMoveSpeed = 3f;
    public float BaseRotationSpeed = 3f;
    public float DodgeForce = 2f;
    public float NegativeDodgeForce = 3f;

    [ReadOnly] public float MovementVelocityDamp;

    [ReadOnly] public Vector3 MouseDelta;
    [ReadOnly] public Vector3 MovementInput;
    [ReadOnly] public Vector3 DodgeVelocity;
    [ReadOnly] public bool IsSlowed;
    [ReadOnly] public Vector3 Velocity;
    [ReadOnly] public Vector3 RelativeVelocity;

    public bool CanBlockOrDodge => Mathf.Abs(DodgeVelocity.sqrMagnitude) < 0.1f;
    public bool IsRolling => Mathf.Abs(DodgeVelocity.sqrMagnitude) > 0.1f;
    public bool IsBlocking;
    public bool BackMoving => RelativeVelocity.z < 0;
}
