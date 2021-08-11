using UnityEngine;

/// <summary>
/// Motor State sent from Client to Server
/// </summary>
public struct ClientPlayerMotorState
{
    public uint FixedFrame;
    public Vector3 MousePosition;
    public float Horizontal;
    public float Forward;
    public bool ActiveAbility;
    public bool IsSlowed;
    public MovementActionCode movementActionCode;
}
