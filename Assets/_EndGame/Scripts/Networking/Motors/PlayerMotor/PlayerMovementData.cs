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
    [ReadOnly] public bool BackMoving = false;

    [ReadOnly] public Vector3 MouseDelta;
    [ReadOnly] public Vector3 MovementInput;
    [ReadOnly] public Vector3 AttackVelocity;
    [ReadOnly] public Vector3 RecoilVelocity;
    [ReadOnly] public Vector3 DodgeVelocity;
    [ReadOnly] public Vector3 Velocity;

    public bool CanBlockOrDodge => Mathf.Abs(DodgeVelocity.magnitude) < 0.1f;
    public bool IsRolling => Mathf.Abs(DodgeVelocity.magnitude) > 0.1f;
}
